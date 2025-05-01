using PVZEngine.Entities;
using PVZEngine.Level;
using MVZ2.GameContent.Armors;

namespace MVZ2.GameContent.Enemies
{
    [EntityBehaviourDefinition(VanillaEnemyNames.MonkZombie)]
    public class MonkZombie : Zombie
    {
        public MonkZombie(string nsp, string name) : base(nsp, name)
        {
        }
    }
}
