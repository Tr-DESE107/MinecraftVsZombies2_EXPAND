using PVZEngine.Entities;
using PVZEngine.Level;

namespace MVZ2.GameContent.Enemies
{
    [EntityBehaviourDefinition(VanillaEnemyNames.MegaZombie)]
    public class MegaZombie : Zombie
    {
        public MegaZombie(string nsp, string name) : base(nsp, name)
        {
        }
    }
}
