using PVZEngine.Entities;
using PVZEngine.Level;

namespace MVZ2.GameContent.Enemies
{
    [EntityBehaviourDefinition(VanillaEnemyNames.SuperMegaZombie)]
    public class SuperMegaZombie : Zombie
    {
        public SuperMegaZombie(string nsp, string name) : base(nsp, name)
        {
        }
    }
}
