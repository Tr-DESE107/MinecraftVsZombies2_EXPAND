using MVZ2.GameContent.Armors;
using MVZ2.GameContent.Models;
using MVZ2.Vanilla;
using PVZEngine.Entities;

namespace MVZ2.GameContent.Enemies
{
    [Definition(VanillaEnemyNames.ironHelmettedZombie)]
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
