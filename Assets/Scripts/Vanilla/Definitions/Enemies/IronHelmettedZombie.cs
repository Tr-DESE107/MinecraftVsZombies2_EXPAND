using MVZ2.GameContent.Armors;
using MVZ2.Vanilla;
using PVZEngine.LevelManagement;

namespace MVZ2.GameContent.Enemies
{
    [Definition(EnemyNames.ironHelmettedZombie)]
    [SpawnDefinition(5)]
    [EntitySeedDefinition(125, VanillaMod.spaceName, RechargeNames.none)]
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
