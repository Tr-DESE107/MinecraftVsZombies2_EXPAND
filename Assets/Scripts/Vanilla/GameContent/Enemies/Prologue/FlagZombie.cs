using MVZ2.GameContent.Buffs.Enemies;
using PVZEngine.Entities;
using PVZEngine.Level;

namespace MVZ2.GameContent.Enemies
{
    [EntityBehaviourDefinition(VanillaEnemyNames.flagZombie)]
    public class FlagZombie : Zombie
    {
        public FlagZombie(string nsp, string name) : base(nsp, name)
        {
        }

        public override void Init(Entity entity)
        {
            base.Init(entity);
            entity.SetAnimationBool("HasFlag", true);
            var speedBuff = entity.GetFirstBuff<RandomEnemySpeedBuff>();
            if (speedBuff != null)
            {
                RandomEnemySpeedBuff.SetSpeed(speedBuff, 2);
            }
        }
    }
}
