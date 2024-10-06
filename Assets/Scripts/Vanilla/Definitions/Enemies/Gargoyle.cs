using MVZ2.Vanilla;
using PVZEngine.Definitions;
using PVZEngine.Level;
using UnityEngine;

namespace MVZ2.GameContent.Enemies
{
    [Definition(VanillaEnemyNames.gargoyle)]
    [SpawnDefinition(0)]
    [EntitySeedDefinition(100, VanillaMod.spaceName, VanillaRechargeNames.none)]
    public class Gargoyle : MeleeEnemy
    {
        public Gargoyle(string nsp, string name) : base(nsp, name)
        {
        }
        public override void Init(Entity entity)
        {
            base.Init(entity);
            entity.InitFragment();
        }
        protected override void UpdateLogic(Entity entity)
        {
            base.UpdateLogic(entity);
            entity.UpdateFragment();
            entity.SetAnimationInt("HealthState", entity.GetHealthState(3));
        }
        public override void PostTakeDamage(DamageResult bodyResult, DamageResult armorResult)
        {
            base.PostTakeDamage(bodyResult, armorResult);
            if (bodyResult != null)
            {
                bodyResult.Entity.AddFragmentTickDamage(bodyResult.Amount);
            }
        }
        public override void PostDeath(Entity entity, DamageInfo info)
        {
            base.PostDeath(entity, info);
            entity.PostFragmentDeath(info);
            entity.Remove();
        }
    }
}
