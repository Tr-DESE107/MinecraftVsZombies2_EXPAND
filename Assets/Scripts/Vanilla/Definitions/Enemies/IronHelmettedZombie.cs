using MVZ2.GameContent.Armors;
using MVZ2.Vanilla;
using PVZEngine;
using UnityEngine;

namespace MVZ2.GameContent.Enemies
{
    [Definition(EnemyNames.ironHelmettedZombie)]
    [EntitySeedDefinition(125, VanillaMod.spaceName, RechargeNames.none)]
    public class IronHelmettedZombie : Zombie
    {
        public override void Init(Entity entity)
        {
            base.Init(entity);
            entity.EquipArmor<IronHelmet>();
        }
    }
}
