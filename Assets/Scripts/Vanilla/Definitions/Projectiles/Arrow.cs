using MVZ2.Vanilla;
using PVZEngine.Definitions;
using PVZEngine.Level;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace MVZ2.GameContent.Projectiles
{
    [Definition(VanillaProjectileNames.arrow)]
    public class Arrow : VanillaProjectile
    {
        public Arrow(string nsp, string name) : base(nsp, name)
        {
            SetProperty(VanillaProjectileProps.POINT_TO_DIRECTION, true);
            SetProperty(EngineEntityProps.SIZE, new Vector3(32, 2, 2));
        }
    }
}
