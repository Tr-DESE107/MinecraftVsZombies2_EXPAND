using MVZ2.Vanilla;
using PVZEngine;
using UnityEngine;

namespace MVZ2.GameContent.Projectiles
{
    [Definition(ProjectileNames.arrow)]
    public class Arrow : VanillaProjectile
    {
        public Arrow(string nsp, string name) : base(nsp, name)
        {
            SetProperty(ProjectileProperties.POINT_TO_DIRECTION, true);
            SetProperty(EntityProperties.SIZE, new Vector3(32, 2, 2));
        }
    }
}
