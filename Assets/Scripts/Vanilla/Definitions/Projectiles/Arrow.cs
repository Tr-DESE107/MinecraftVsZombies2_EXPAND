using MVZ2.Vanilla;
using PVZEngine.Definitions;
using PVZEngine.Level;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace MVZ2.GameContent.Projectiles
{
    [Definition(ProjectileNames.arrow)]
    public class Arrow : VanillaProjectile
    {
        public Arrow(string nsp, string name) : base(nsp, name)
        {
            SetProperty(ProjectileProps.POINT_TO_DIRECTION, true);
            SetProperty(EntityProperties.SIZE, new Vector3(32, 2, 2));
        }
    }
}
