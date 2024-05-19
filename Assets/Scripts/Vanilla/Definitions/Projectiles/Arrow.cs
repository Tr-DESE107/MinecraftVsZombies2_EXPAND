using MVZ2.Vanilla;
using PVZEngine;

namespace MVZ2.GameContent.Projectiles
{
    public class Arrow : VanillaProjectile
    {
        public Arrow() : base()
        {
            propertyDict.SetProperty(ProjectileProperties.POINT_TO_DIRECTION, true);
        }
    }
}
