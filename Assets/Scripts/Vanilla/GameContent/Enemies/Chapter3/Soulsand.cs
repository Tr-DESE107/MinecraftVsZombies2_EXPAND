using MVZ2.Vanilla.Entities;
using PVZEngine.Damages;
using PVZEngine.Entities;
using PVZEngine.Level;
using UnityEngine;

namespace MVZ2.GameContent.Enemies
{
    [EntityBehaviourDefinition(VanillaEnemyNames.soulsand)]
    public class Soulsand : EnemyBehaviour
    {
        public Soulsand(string nsp, string name) : base(nsp, name)
        {
        }
        public override void Init(Entity entity)
        {
            base.Init(entity);
            entity.InitFragment();
            entity.Timeout = entity.GetMaxTimeout();
        }
        protected override void UpdateLogic(Entity entity)
        {
            base.UpdateLogic(entity);
            entity.UpdateFragment();

            if (entity.Velocity.y != 0)
            {
                entity.AddFragmentTickDamage(Mathf.Abs(entity.Velocity.y));
            }
            entity.SetAnimationInt("HealthState", entity.GetHealthState(3));
            if (entity.Timeout >= 0)
            {
                entity.Timeout--;
                if (entity.Timeout <= 0)
                {
                    entity.Die(entity);
                }
            }
        }
        public override void PostTakeDamage(DamageOutput result)
        {
            base.PostTakeDamage(result);
            var entity = result.Entity;
            var bodyResult = result.BodyResult;
            if (bodyResult != null)
            {
                entity.AddFragmentTickDamage(bodyResult.Amount);
            }
        }
        public override void PostDeath(Entity entity, DeathInfo info)
        {
            base.PostDeath(entity, info);
            entity.PostFragmentDeath(info);
            entity.Remove();
        }
    }
}
