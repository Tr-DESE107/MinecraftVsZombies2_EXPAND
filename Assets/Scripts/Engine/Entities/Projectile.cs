using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PVZEngine
{
    public class Projectile : Entity
    {
        #region 公有方法

        #region 构造器
        public Projectile(Game level, int id, int seed) : base(level, id, seed)
        {

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
            if (WillDestroyOutsideLawn() && IsOutsideView())
            {
                Remove();
                return;
            }
            if (Timeout <= 0)
            {
                Remove();
                return;
            }
        }

        public override void OnEntityCollisionEnter(Entity other, bool actively)
        {
            UnitCollide(other);
        }

        public override void OnEntityCollisionStay(Entity other, bool actively)
        {
            UnitCollide(other);
        }

        public override void OnEntityCollisionExit(Entity other, bool actively)
        {
            UnitExit(other);
            if (other == SpawnerReference.Entity)
            {
                canHitSpawner = true;
            }
        }
        #endregion

        #region 属性
        public int GetMaxTimeout()
        {
            return GetProperty<int>(ProjectileProperties.MAX_TIMEOUT);
        }
        public bool WillDestroyOutsideLawn()
        {
            return !GetProperty<bool>(ProjectileProperties.NO_DESTROY_OUTSIDE_LAWN);
        }
        public bool IsPiercing()
        {
            return GetProperty<bool>(ProjectileProperties.PIERCING);
        }
        #endregion

        public virtual bool IsOutsideView()
        {
            var bounds = GetBounds();
            return
                Pos.x < 180 - bounds.extents.x ||
                Pos.x > 1100 + bounds.extents.x ||
                Pos.z > 550 ||
                Pos.z < -50 ||
                Pos.y > 1000 ||
                Pos.y < -1000;
        }

        public override void StopWarpingLane()
        {
            base.StopWarpingLane();
            var vel = Velocity;
            vel.z = 0;
            Velocity = vel;
        }

        public bool CanPierce(Entity other)
        {
            bool ethereal = other.EquipedArmor.Exists() ? false : other.IsEthereal();
            return ethereal || IsPiercing();
        }
        #endregion

        #region 私有方法
        private void UnitCollide(Entity other)
        {
            // 是否可以击中发射者。
            if (SpawnerReference != null && other == SpawnerReference.Entity && !canHitSpawner)
                return;

            if (WaitingDestroy || !IsEnemy(other) || collided.Any(c => c.ID == other.ID))
                return;

            if (!Detection.IsZCoincide(Pos.z, GetScaledSize().z, other.Pos.z, other.GetScaledSize().z))
                return;

            other.TakeDamage(GetDamage(), new DamageEffectList(), new EntityReference(this));

            collided.Add(new EntityReference(other));
            if (!CanPierce(other))
            {
                Remove();
                return;
            }
        }

        private void UnitExit(Entity other)
        {
            collided.RemoveAll(c => c.ID == other.ID);
        }
        #endregion
        public override int Type => EntityTypes.PROJECTILE;
        public int Timeout { get; set; }
        private List<EntityReference> collided = new List<EntityReference>();
        private bool canHitSpawner;

    }
}