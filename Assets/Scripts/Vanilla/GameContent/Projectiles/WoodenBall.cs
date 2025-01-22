using MVZ2.Vanilla;
using MVZ2.Vanilla.Entities;
using PVZEngine.Entities;
using UnityEngine;

namespace MVZ2.GameContent.Projectiles
{
    [Definition(VanillaProjectileNames.woodenBall)]
    public class WoodenBall : ProjectileBehaviour
    {
        public WoodenBall(string nsp, string name) : base(nsp, name)
        {
        }
        public override void Update(Entity projectile)
        {
            base.Update(projectile);
            float angleSpeed = -projectile.Velocity.x * 2.5f;
            projectile.RenderRotation += Vector3.forward * angleSpeed;
        }
    }
}
