using System.Collections.Generic;
using MVZ2.GameContent.Buffs.Enemies;
using MVZ2.GameContent.Damages;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Properties;
using PVZEngine.Callbacks;
using PVZEngine.Damages;
using PVZEngine.Entities;
using PVZEngine.Level;
using UnityEngine;

namespace MVZ2.GameContent.Enemies
{
    [EntityBehaviourDefinition(VanillaEnemyNames.shikaisenStaff)]
    public class ShikaisenStaff : EnemyBehaviour
    {
        public ShikaisenStaff(string nsp, string name) : base(nsp, name)
        {
            AddTrigger(LevelCallbacks.POST_ENTITY_DEATH, PostEnemyDeathCallback, filter: EntityTypes.ENEMY);
        }
        public override void Init(Entity entity)
        {
            base.Init(entity);
            SetTargetPosition(entity, entity.Position);
            entity.Timeout = entity.GetMaxTimeout();
        }
        protected override void UpdateLogic(Entity entity)
        {
            base.UpdateLogic(entity);
            var targetPosition = GetTargetPosition(entity);
            targetPosition.y = entity.Level.GetGroundY(targetPosition.x, targetPosition.z);
            entity.Position = targetPosition;
            entity.Velocity = Vector3.zero;
            entity.StopChangingLane();

            entity.SetAnimationFloat("Range", entity.GetRange());
            entity.SetModelDamagePercent();
            if (entity.Timeout >= 0)
            {
                entity.Timeout--;
                if (entity.Timeout <= 0)
                {
                    entity.Die(entity);
                }
            }
        }
        public override void PreTakeDamage(DamageInput input, CallbackResult result)
        {
            base.PreTakeDamage(input, result);
            var entity = input.Entity;
            if (input.HasEffect(VanillaDamageEffects.FIRE))
            {
                input.SetAmount(entity.GetMaxHealth() * 10);
            }
        }
        private void PostEnemyDeathCallback(LevelCallbacks.PostEntityDeathParams param, CallbackResult result)
        {
            var entity = param.entity;
            var info = param.deathInfo;
            if (info.HasEffect(VanillaDamageEffects.REMOVE_ON_DEATH) || info.HasEffect(VanillaDamageEffects.DROWN))
                return;
            var staff = entity.Level.FindFirstEntity(e => e.IsEntityOf(VanillaEnemyID.shikaisenStaff) && (e.Position - entity.Position).magnitude <= e.GetRange() && e.ExistsAndAlive());
            if (staff == null)
                return;
            var buff = entity.CreateBuff<ShikaisenReviveBuff>();
            ShikaisenReviveBuff.SetSource(buff, new EntityID(staff));
            entity.AddBuff(buff);
        }
        public override void PostDeath(Entity entity, DeathInfo info)
        {
            base.PostDeath(entity, info);
            entity.Remove();
        }
        public static Vector3 GetTargetPosition(Entity enemy) => enemy.GetBehaviourField<Vector3>(PROP_TARGET_POSITION);
        public static void SetTargetPosition(Entity enemy, Vector3 value) => enemy.SetBehaviourField(PROP_TARGET_POSITION, value);
        public static readonly VanillaEntityPropertyMeta<Vector3> PROP_TARGET_POSITION = new VanillaEntityPropertyMeta<Vector3>("TargetPosition");
        private List<Entity> detectBuffer = new List<Entity>(1024);
    }
}
