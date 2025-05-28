using MVZ2.Vanilla.Entities;
using PVZEngine.Entities;
using PVZEngine.Level;

namespace MVZ2.GameContent.Projectiles
{
    [EntityBehaviourDefinition(VanillaProjectileNames.bullet)]
    public class Bullet : ProjectileBehaviour
    {
        public Bullet(string nsp, string name) : base(nsp, name)
        {
        }
    }
}
