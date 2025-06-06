using MVZ2.GameContent.Buffs.Enemies;
using MVZ2.Vanilla.Entities;
using PVZEngine.Entities;
using PVZEngine.Level;

namespace MVZ2.GameContent.Enemies
{
    [EntityBehaviourDefinition(VanillaEnemyNames.HostZombie)]
    public class HostZombie : Zombie
    {
        public HostZombie(string nsp, string name) : base(nsp, name)
        {
        }

        public override void Init(Entity entity)
        {
            base.Init(entity);
            entity.InflictRegenerationBuff(2f, 60000);
        }

    }
}
