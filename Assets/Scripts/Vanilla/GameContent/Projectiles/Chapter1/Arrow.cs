using MVZ2.Vanilla.Entities;
using PVZEngine.Level;

namespace MVZ2.GameContent.Projectiles
{
    [EntityBehaviourDefinition(VanillaProjectileNames.arrow)]
    public class Arrow : ProjectileBehaviour
    {
        public Arrow(string nsp, string name) : base(nsp, name)
        {
        }
    }
}
