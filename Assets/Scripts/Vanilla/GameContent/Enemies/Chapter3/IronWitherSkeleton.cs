using MVZ2.GameContent.Armors;
using MVZ2.Vanilla.Entities;
using PVZEngine.Entities;
using PVZEngine.Level;

namespace MVZ2.GameContent.Enemies
{
    [EntityBehaviourDefinition(VanillaEnemyNames.IronWitherSkeleton)]
    public class IronWitherSkeleton : WitherSkeleton
    {
        public IronWitherSkeleton(string nsp, string name) : base(nsp, name)
        {
        }

        public override void Init(Entity entity)
        {
            base.Init(entity);
            entity.EquipMainArmor(VanillaArmorID.ironHelmet);
        }
    }
}
