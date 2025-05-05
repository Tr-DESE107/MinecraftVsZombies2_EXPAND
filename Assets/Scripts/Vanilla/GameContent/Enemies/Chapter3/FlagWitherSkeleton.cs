using PVZEngine.Entities;
using PVZEngine.Level;
using MVZ2.GameContent.Armors;

namespace MVZ2.GameContent.Enemies
{
    [EntityBehaviourDefinition(VanillaEnemyNames.FlagWitherSkeleton)]
    public class FlagWitherSkeleton : WitherSkeleton
    {
        public FlagWitherSkeleton(string nsp, string name) : base(nsp, name)
        {
        }

        public override void Init(Entity entity)
        {
            base.Init(entity);
            entity.SetAnimationBool("HasFlag", true);
            entity.EquipArmor<IronHelmet>();
        }
        protected override float GetRandomSpeedMultiplier(Entity entity)
        {
            return 2;
        }
    }
}
