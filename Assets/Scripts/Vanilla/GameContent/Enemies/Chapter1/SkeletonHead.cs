using MVZ2.GameContent.Buffs.Enemies;
using MVZ2.Vanilla.Enemies;
using PVZEngine.Buffs;
using PVZEngine.Entities;
using PVZEngine.Level;

namespace MVZ2.GameContent.Enemies
{
    [EntityBehaviourDefinition(VanillaEnemyNames.SkeletonHead)]
    public class SkeletonHead : MeleeEnemy
    {
        public SkeletonHead(string nsp, string name) : base(nsp, name)
        {
        }

        public override void Init(Entity entity)
        {
            base.Init(entity);
            var fly = entity.AddBuff<FlyBuff>();
            fly.SetProperty(FlyBuff.PROP_TARGET_HEIGHT, 20f);
        }

    }
}
