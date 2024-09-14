using MVZ2.Vanilla;
using PVZEngine.LevelManagement;

namespace MVZ2.GameContent.Enemies
{
    [Definition(EnemyNames.flagZombie)]
    [SpawnDefinition(0)]
    [EntitySeedDefinition(50, VanillaMod.spaceName, RechargeNames.none)]
    public class FlagZombie : Zombie
    {
        public FlagZombie(string nsp, string name) : base(nsp, name)
        {
        }

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
