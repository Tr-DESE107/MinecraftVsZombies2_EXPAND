using MVZ2.Vanilla.Entities;
using PVZEngine.Level;

namespace MVZ2.GameContent.Projectiles
{
    [EntityBehaviourDefinition(VanillaProjectileNames.Poisonball)]
    public class Poisonball : ProjectileBehaviour
    {
        public Poisonball(string nsp, string name) : base(nsp, name)
        {
        }
    }
}
