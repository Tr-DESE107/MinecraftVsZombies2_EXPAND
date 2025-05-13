using System.Collections.Generic;
using MVZ2.GameContent.Damages;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Callbacks;
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
            AddTrigger(VanillaLevelCallbacks.PRE_ENEMY_FAINT, PreEnemyFaintCallback);
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

            entity.SetAnimationFloat("Range", entity.GetRange());
            entity.SetAnimationInt("HealthState", entity.GetHealthState(5));
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
        private void PreEnemyFaintCallback(EntityCallbackParams param, CallbackResult result)
        {
            var entity = param.entity;
            var staff = entity.Level.FindFirstEntity(e => e.IsEntityOf(VanillaEnemyID.shikaisenStaff) && (e.Position - entity.Position).magnitude <= e.GetRange() && e.ExistsAndAlive());
            if (staff == null)
                return;
            entity.Revive();
            var costHealth = Mathf.Min(staff.Health, entity.GetMaxHealth());
            entity.HealEffects(costHealth, staff);
            staff.TakeDamage(costHealth, new DamageEffectList(VanillaDamageEffects.SELF_DAMAGE), staff);
            if (staff.Health <= 0)
            {
                staff.Die();
            }
            result.SetFinalValue(false);

            entity.PlaySound(VanillaSoundID.revived);
        }
        public override void PostDeath(Entity entity, DeathInfo info)
        {
            base.PostDeath(entity, info);
            entity.Remove();
        }
        public static Vector3 GetTargetPosition(Entity enemy) => enemy.GetBehaviourField<Vector3>(PROP_TARGET_POSITION);
        public static void SetTargetPosition(Entity enemy, Vector3 value) => enemy.SetBehaviourField(PROP_TARGET_POSITION, value);
        public static readonly VanillaEntityPropertyMeta PROP_TARGET_POSITION = new VanillaEntityPropertyMeta("TargetPosition");
        private List<Entity> detectBuffer = new List<Entity>(1024);
    }
}
