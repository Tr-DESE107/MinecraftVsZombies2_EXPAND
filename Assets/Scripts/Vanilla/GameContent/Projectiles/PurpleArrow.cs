using MVZ2.Vanilla.Entities;
using PVZEngine.Level;

namespace MVZ2.GameContent.Projectiles
{
    [EntityBehaviourDefinition(VanillaProjectileNames.purpleArrow)]
    public class purpleArrow : ProjectileBehaviour
    {
        public purpleArrow(string nsp, string name) : base(nsp, name)
        {
        }
    }
}
