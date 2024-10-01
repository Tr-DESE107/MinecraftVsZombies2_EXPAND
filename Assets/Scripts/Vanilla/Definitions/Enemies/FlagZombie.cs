using MVZ2.Vanilla;
using PVZEngine.Level;

namespace MVZ2.GameContent.Enemies
{
    [Definition(VanillaEnemyNames.flagZombie)]
    [SpawnDefinition(0)]
    [EntitySeedDefinition(50, VanillaMod.spaceName, VanillaRechargeNames.none)]
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
