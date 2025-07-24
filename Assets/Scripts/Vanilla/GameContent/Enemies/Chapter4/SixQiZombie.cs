using MVZ2.GameContent.Buffs.Enemies;
using MVZ2.Vanilla.Entities;
using PVZEngine.Entities;
using PVZEngine.Level;

namespace MVZ2.GameContent.Enemies
{
    [EntityBehaviourDefinition(VanillaEnemyNames.SixQiZombie)]
    public class SixQiZombie : Zombie
    {
        public SixQiZombie(string nsp, string name) : base(nsp, name)
        {
        }

        public override void Init(Entity entity)
        {
            base.Init(entity);
            if (!entity.HasBuff<SixQiResistanceBuff>())
            {
                entity.AddBuff<SixQiResistanceBuff>();
            }
            
        }

    }
}
