using MVZ2.Vanilla.Entities;
using PVZEngine.Level;

namespace MVZ2.GameContent.Projectiles
{
    [EntityBehaviourDefinition(VanillaProjectileNames.spike)]
    public class Spike : ProjectileBehaviour
    {
        public Spike(string nsp, string name) : base(nsp, name)
        {
        }
    }
}
