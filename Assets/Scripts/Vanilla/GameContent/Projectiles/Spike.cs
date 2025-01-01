using MVZ2.Vanilla;
using MVZ2.Vanilla.Entities;

namespace MVZ2.GameContent.Projectiles
{
    [Definition(VanillaProjectileNames.spike)]
    public class Spike : ProjectileBehaviour
    {
        public Spike(string nsp, string name) : base(nsp, name)
        {
        }
    }
}
