using MVZ2.Vanilla;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Level;
using PVZEngine.Damages;
using PVZEngine.Entities;

namespace MVZ2.GameContent.Projectiles
{
    [Definition(VanillaProjectileNames.breakoutPearl)]
    public class BreakoutPearl : ProjectileBehaviour
    {
        public BreakoutPearl(string nsp, string name) : base(nsp, name)
        {
        }
        public override void Init(Entity entity)
        {
            base.Init(entity);
            entity.Timeout = 30;
        }
        protected override void PostHitEntity(ProjectileHitOutput hitResult, DamageOutput damage)
        {
            base.PostHitEntity(hitResult, damage);

            var bullet = hitResult.Projectile;
            var target = hitResult.Other;
            var bullet2Target = bullet.Position - target.Position;
            var newDirection = bullet2Target;
            newDirection.y = 0;
            var velocity = bullet.Velocity;
            var newVelocity = newDirection.normalized * velocity.magnitude;
            velocity.x = newVelocity.x;
            velocity.y = 0;
            velocity.z = newVelocity.z;
            bullet.Velocity = velocity;
        }
        public override void Update(Entity projectile)
        {
            base.Update(projectile);
            projectile.Timeout = 30;
            var level = projectile.Level;
            var position = projectile.Position;
            var velocity = projectile.Velocity;
            if (position.x > MAX_X)
            {
                position.x = MAX_X;
                velocity.x *= -1;
            }
            if (position.z > level.GetGridTopZ())
            {
                position.z = level.GetGridTopZ();
                velocity.z *= -1;
            }
            else if (position.z < level.GetGridBottomZ())
            {
                position.z = level.GetGridBottomZ();
                velocity.z *= -1;
            }
            projectile.Position = position;
            projectile.Velocity = velocity;
        }

        public const float MAX_X = VanillaLevelExt.RIGHT_BORDER - 40;
    }
}
