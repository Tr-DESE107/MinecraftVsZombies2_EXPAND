using MVZ2.GameContent.Armors;
using MVZ2.GameContent.Recharges;
using MVZ2.Vanilla;
using MVZ2.Vanilla.Level;
using MVZ2.Vanilla.SeedPacks;
using PVZEngine.Entities;

namespace MVZ2.GameContent.Enemies
{
    [Definition(VanillaEnemyNames.ironHelmettedZombie)]
    [SpawnDefinition(5)]
    [EntitySeedDefinition(125, VanillaMod.spaceName, VanillaRechargeNames.none)]
    public class IronHelmettedZombie : Zombie
    {
        public IronHelmettedZombie(string nsp, string name) : base(nsp, name)
        {
        }

        public override void Init(Entity entity)
        {
            base.Init(entity);
            entity.EquipArmor<IronHelmet>();
        }
    }
}
