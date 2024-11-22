using MVZ2.GameContent.Recharges;
using MVZ2.Vanilla;
using MVZ2.Vanilla.Enemies;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Level;
using MVZ2.Vanilla.SeedPacks;
using PVZEngine.Definitions;
using PVZEngine.Entities;

namespace MVZ2.GameContent.Enemies
{
    [Definition(VanillaEnemyNames.zombie)]
    [SpawnDefinition(1, previewCount: 3)]
    [EntitySeedDefinition(50, VanillaMod.spaceName, VanillaRechargeNames.none)]
    public class Zombie : MeleeEnemy
    {
        public Zombie(string nsp, string name) : base(nsp, name)
        {
        }
        protected override void UpdateLogic(Entity entity)
        {
            base.UpdateLogic(entity);
            entity.SetAnimationInt("HealthState", entity.GetHealthState(2));
        }
    }
}
