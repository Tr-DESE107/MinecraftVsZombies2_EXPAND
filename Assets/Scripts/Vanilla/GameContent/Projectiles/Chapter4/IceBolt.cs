using MVZ2.Vanilla.Entities;
using PVZEngine.Entities;
using PVZEngine.Level;

namespace MVZ2.GameContent.Projectiles
{
    [EntityBehaviourDefinition(VanillaProjectileNames.iceBolt)]
    public class IceBolt : ProjectileBehaviour
    {
        public IceBolt(string nsp, string name) : base(nsp, name)
        {
        }
    }
}
