using MVZ2.GameContent.Armors;
using PVZEngine.Entities;
using PVZEngine.Level;

namespace MVZ2.GameContent.Enemies
{
    [EntityBehaviourDefinition(VanillaEnemyNames.LeatherWitherSkeleton)]
    public class LeatherWitherSkeleton : WitherSkeleton
    {
        public LeatherWitherSkeleton(string nsp, string name) : base(nsp, name)
        {
        }

        public override void Init(Entity entity)
        {
            base.Init(entity);
            entity.EquipArmor<LeatherCap>();
        }
    }
}
