using MVZ2.Vanilla.Entities;
using PVZEngine.Level;

namespace MVZ2.GameContent.Projectiles
{
    [EntityBehaviourDefinition(VanillaProjectileNames.snowball)]
    public class Snowball : ProjectileBehaviour
    {
        public Snowball(string nsp, string name) : base(nsp, name)
        {
        }
    }
}
