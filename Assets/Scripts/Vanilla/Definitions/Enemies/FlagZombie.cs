using MVZ2.GameContent.Buffs;
using MVZ2.Vanilla;
using PVZEngine;

namespace MVZ2.GameContent.Enemies
{
    [Definition(EnemyNames.flagZombie)]
    [EntitySeedDefinition(50, VanillaMod.spaceName, RechargeNames.none)]
    public class FlagZombie : Zombie
    {
        public override void Init(Entity entity)
        {
            base.Init(entity);
            entity.SetAnimationBool("HasFlag", true);
        }
        protected override float GetRandomSpeedMultiplier(Entity entity)
        {
            return 2;
        }
    }
}
