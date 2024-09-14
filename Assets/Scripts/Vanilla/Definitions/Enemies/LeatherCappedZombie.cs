using MVZ2.GameContent.Armors;
using MVZ2.Vanilla;
using PVZEngine.Level;

namespace MVZ2.GameContent.Enemies
{
    [Definition(EnemyNames.leatherCappedZombie)]
    [SpawnDefinition(2)]
    [EntitySeedDefinition(75, VanillaMod.spaceName, RechargeNames.none)]
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
