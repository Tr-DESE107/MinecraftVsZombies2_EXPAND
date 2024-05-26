using System.Collections.Generic;
using System.Linq;
using PVZEngine.Serialization;
using UnityEngine;

namespace PVZEngine
{
    public class Projectile : Entity
    {
        #region 公有方法

        #region 构造器
        internal Projectile(Game level) : base(level)
        {
        }
        public Projectile(Game level, int id, EntityDefinition definition, int seed) : base(level, id, definition, seed)
        {
            CollisionMask = EntityCollision.MASK_CONTRAPTION
                | EntityCollision.MASK_ENEMY
                | EntityCollision.MASK_OBSTACLE
                | EntityCollision.MASK_BOSS
                | EntityCollision.MASK_HOSTILE;
        }
        #endregion

        #region 生命周期
        protected override void OnInit(Entity spawner)
        {
            base.OnInit(spawner);
            ShadowScale = new Vector3(0.5f, 0.5f, 1);
            Timeout = GetMaxTimeout();
            WarpLaneSpeed = 1;
        }
        protected override void OnUpdate()
        {
            base.OnUpdate();
            Timeout--;
            if (Timeout <= 0)
            {
                Remove();
                return;
            }
        }

        protected override void OnCollision(Entity other, int state)
        {
            if (state != EntityCollision.STATE_EXIT)
            {
                UnitCollide(other);
            }
            else
            {
                UnitExit(other);
                var spawner = SpawnerReference?.GetEntity(Game);
                if (other == spawner)
                {
                    canHitSpawner = true;
                }
            }
        }
        #endregion

        #region 属性
        public int GetMaxTimeout()
        {
            return GetProperty<int>(ProjectileProperties.MAX_TIMEOUT);
        }
        public bool IsPiercing()
        {
            return GetProperty<bool>(ProjectileProperties.PIERCING);
        }
        #endregion

        public override void StopWarpingLane()
        {
            base.StopWarpingLane();
            var vel = Velocity;
            vel.z = 0;
            Velocity = vel;
        }

        public bool CanPierce(Entity other)
        {
            bool ethereal = Armor.Exists(other.EquipedArmor) ? false : other.IsEthereal();
            return ethereal || IsPiercing();
        }
        #endregion

        #region 私有方法
        protected override SerializableEntity CreateSerializableEntity() => new SerializableProjectile();
        protected override void ApplySerialize(SerializableEntity seri)
        {
            base.ApplySerialize(seri);
            var seriProj = seri as SerializableProjectile;
            seriProj.canHitSpawner = canHitSpawner;
            seriProj.collided = collided.ConvertAll(c => c.ID);
        }
        public override void ApplyDeserialize(SerializableEntity seri)
        {
            base.ApplyDeserialize(seri);
            var seriProj = seri as SerializableProjectile;
            canHitSpawner = seriProj.canHitSpawner;
            collided = seriProj.collided.ConvertAll(c => Game.FindEntityByID(c));
        }
        private void UnitCollide(Entity other)
        {
            // 是否可以击中发射者。
            var spawner = SpawnerReference?.GetEntity(Game);
            if (other == spawner && !canHitSpawner)
                return;

            if (Removed || !IsEnemy(other) || collided.Any(c => c.ID == other.ID) || other.IsDead)
                return;

            if (!Detection.IsZCoincide(Pos.z, GetScaledSize().z, other.Pos.z, other.GetScaledSize().z))
                return;

            other.TakeDamage(GetDamage(), new DamageEffectList(), new EntityReference(this));

            collided.Add(other);
            if (!CanPierce(other))
            {
                Remove();
                return;
            }
        }

        private void UnitExit(Entity other)
        {
            collided.Remove(other);
        }
        #endregion
        public override int Type => EntityTypes.PROJECTILE;
        private List<Entity> collided = new List<Entity>();
        private bool canHitSpawner;
    }
}