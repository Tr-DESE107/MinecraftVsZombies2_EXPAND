using MVZ2.GameContent.Armors;
using MVZ2.Vanilla;
using PVZEngine.Level;

namespace MVZ2.GameContent.Enemies
{
    [Definition(VanillaEnemyNames.leatherCappedZombie)]
    [SpawnDefinition(2, previewCount: 2)]
    [EntitySeedDefinition(75, VanillaMod.spaceName, VanillaRechargeNames.none)]
    public class LeatherCappedZombie : Zombie
    {
        public LeatherCappedZombie(string nsp, string name) : base(nsp, name)
        {
        }

        public override void Init(Entity entity)
        {
            base.Init(entity);
            entity.EquipArmor<LeatherCap>();
        }
    }
}
