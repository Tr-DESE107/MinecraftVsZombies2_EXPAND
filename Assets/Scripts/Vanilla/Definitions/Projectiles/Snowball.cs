using MVZ2.Vanilla;
using PVZEngine.Definitions;
using UnityEngine;

namespace MVZ2.GameContent.Projectiles
{
    [Definition(ProjectileNames.snowball)]
    public class Snowball : VanillaProjectile
    {
        public Snowball(string nsp, string name) : base(nsp, name)
        {
            SetProperty(EntityProperties.SIZE, new Vector3(24, 24, 24));
        }
    }
}
