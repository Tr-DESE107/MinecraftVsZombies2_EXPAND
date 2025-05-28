using PVZEngine.Entities;
using PVZEngine.Level;

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
