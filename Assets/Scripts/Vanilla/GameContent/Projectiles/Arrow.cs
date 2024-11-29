using MVZ2.Vanilla;
using MVZ2.Vanilla.Entities;

namespace MVZ2.GameContent.Projectiles
{
    [Definition(VanillaProjectileNames.arrow)]
    public class Arrow : ProjectileBehaviour
    {
        public Arrow(string nsp, string name) : base(nsp, name)
        {
        }
    }
}
