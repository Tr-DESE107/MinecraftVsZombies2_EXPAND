using MVZ2.Vanilla.Entities;
using PVZEngine.Level;

namespace MVZ2.GameContent.Projectiles
{
    [EntityBehaviourDefinition(VanillaProjectileNames.crossbowBolt)]
    public class CrossbowBolt : ProjectileBehaviour
    {
        public CrossbowBolt(string nsp, string name) : base(nsp, name)
        {
        }
    }
}
