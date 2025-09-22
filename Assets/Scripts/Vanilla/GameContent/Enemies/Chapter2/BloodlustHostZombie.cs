using MVZ2.Vanilla.Entities;
using PVZEngine.Entities;
using PVZEngine.Level;

namespace MVZ2.GameContent.Enemies
{
    [EntityBehaviourDefinition(VanillaEnemyNames.BloodlustHostZombie)]
    public class BloodlustHostZombie : Zombie
    {
        public BloodlustHostZombie(string nsp, string name) : base(nsp, name)
        {
        }

        public override void Init(Entity entity)
        {
            base.Init(entity);
            entity.InflictRegenerationBuff(3f, 60000, new EntitySourceReference(entity));
        }

    }
}
