using MVZ2.Vanilla;
using PVZEngine.Definitions;
using PVZEngine.Level;
using UnityEngine;

namespace MVZ2.GameContent.Projectiles
{
    [Definition(VanillaProjectileNames.snowball)]
    public class Snowball : VanillaProjectile
    {
        public Snowball(string nsp, string name) : base(nsp, name)
        {
        }
    }
}
