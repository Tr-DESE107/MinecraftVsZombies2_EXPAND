using PVZEngine.Entities;
using PVZEngine.Level;

namespace MVZ2.GameContent.Enemies
{
    [EntityBehaviourDefinition(VanillaEnemyNames.LeatherMonkZombie)]
    public class LeatherMonkZombie : Zombie
    {
        public LeatherMonkZombie(string nsp, string name) : base(nsp, name)
        {
        }
    }
}
