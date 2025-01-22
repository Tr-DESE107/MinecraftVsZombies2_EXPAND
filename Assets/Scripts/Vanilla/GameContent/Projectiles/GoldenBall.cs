using MVZ2.GameContent.Pickups;
using MVZ2.Vanilla;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Entities;
using PVZEngine.Damages;
using PVZEngine.Entities;
using UnityEngine;

namespace MVZ2.GameContent.Projectiles
{
    [Definition(VanillaProjectileNames.goldenBall)]
    public class GoldenBall : ProjectileBehaviour
    {
        public GoldenBall(string nsp, string name) : base(nsp, name)
        {
        }
        public override void Update(Entity projectile)
        {
            base.Update(projectile);
            float angleSpeed = -projectile.Velocity.x * 2.5f;
            projectile.RenderRotation += Vector3.forward * angleSpeed;
        }
        protected override void PostHitEntity(ProjectileHitOutput hitResult, DamageOutput damage)
        {
            base.PostHitEntity(hitResult, damage);
            var projectile = hitResult.Projectile;
            projectile.Produce(VanillaPickupID.emerald);

            var vel = projectile.Velocity;
            var vel2D = new Vector2(vel.x, vel.z);
            var speed = vel2D.magnitude;
            vel.x = Mathf.Sign(vel.x) * speed * 0.5f;

            var lane = projectile.GetLane();
            var zSpeed = speed / 2 * Mathf.Sqrt(3);
            int zDir;
            if (lane <= 0)
            {
                zDir = -1;
            }
            else if (lane >= projectile.Level.GetMaxLaneCount() - 1)
            {
                zDir = 1;
            }
            else
            {
                zDir = projectile.RNG.Next(2) * 2 - 1;
            }
            vel.z = zDir * zSpeed;
            projectile.Velocity = vel;
        }
    }
}
