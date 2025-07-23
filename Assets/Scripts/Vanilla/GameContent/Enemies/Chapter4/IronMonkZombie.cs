using PVZEngine.Entities;
using PVZEngine.Level;

namespace MVZ2.GameContent.Enemies
{
    [EntityBehaviourDefinition(VanillaEnemyNames.IronMonkZombie)]
    public class IronMonkZombie : Zombie
    {
        public IronMonkZombie(string nsp, string name) : base(nsp, name)
        {
        }
    }
}
