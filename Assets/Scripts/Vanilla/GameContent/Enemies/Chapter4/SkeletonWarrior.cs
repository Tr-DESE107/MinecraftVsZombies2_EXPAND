using MVZ2.GameContent.Armors;
using MVZ2.GameContent.Buffs;
using MVZ2.Vanilla.Enemies;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Level;
using PVZEngine.Entities;
using PVZEngine.Level;

namespace MVZ2.GameContent.Enemies
{
    [EntityBehaviourDefinition(VanillaEnemyNames.skeletonWarrior)]
    public class SkeletonWarrior : MeleeEnemy
    {
        public SkeletonWarrior(string nsp, string name) : base(nsp, name)
        {
        }
        public override void Init(Entity entity)
        {
            base.Init(entity);
            if (entity.Level.IsIZombie())
            {
                entity.AddBuff<IZombieSkeletonWarriorBuff>();
                var helmet = entity.GetMainArmor();
                var shield = entity.GetArmorAtSlot(VanillaArmorSlots.shield);
                helmet?.AddBuff<IZombieSkeletonWarriorArmorBuff>();
                shield?.AddBuff<IZombieSkeletonWarriorArmorBuff>();
            }
        }
        protected override void UpdateLogic(Entity entity)
        {
            base.UpdateLogic(entity);
            entity.SetModelHealthStateByCount(2);
        }
    }
}
