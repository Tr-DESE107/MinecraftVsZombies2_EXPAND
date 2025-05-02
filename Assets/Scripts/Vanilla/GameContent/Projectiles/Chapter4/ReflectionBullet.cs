using MVZ2.Vanilla.Entities;
using PVZEngine.Entities;
using PVZEngine.Level;

namespace MVZ2.GameContent.Projectiles
{
    [EntityBehaviourDefinition(VanillaProjectileNames.reflectionBullet)]
    public class ReflectionBullet : ProjectileBehaviour
    {
        public ReflectionBullet(string nsp, string name) : base(nsp, name)
        {
        }
    }
}