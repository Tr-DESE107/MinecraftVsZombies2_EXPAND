using MVZ2.Vanilla;
using PVZEngine;
using UnityEngine;

namespace MVZ2.GameContent.Contraptions
{
    public abstract class DispenserFamily : VanillaContraption
    {
        protected virtual Vector3 ProjectileOffset => projectileOffset;
        protected virtual Vector3 ProjectileVelocity => projectileVelocity;
        protected virtual float ProjectileSizeY => 0.02f;
        protected virtual float ProjectileSizeZ => 0.02f;
        protected virtual float Range => -1;
        protected virtual NamespaceID ShootSound => SoundID.shot;
        protected virtual NamespaceID Projectile => ProjectileID.arrow;
        private Vector3 projectileOffset = new Vector3(25, 30, 0);
        private Vector3 projectileVelocity = new Vector3(10, 0, 0);
        private Detector detector;

        public DispenserFamily()
        {
             detector = new DispenserDetector()
             {
                 ignoreHighEnemy = true,
                 range = Range,
                 shootOffset = ProjectileOffset,
                 projectileSizeY = ProjectileSizeY,
                 projectileSizeZ = ProjectileSizeZ
             };
        }

        public void InitShootTimer(Entity entity)
        {
            var shootTimer = new FrameTimer(entity.RNG.Next(40, 45));
            SetShootTimer(entity, shootTimer);
        }

        public void ShootTick(Entity entity)
        {
            var shootTimer = GetShootTimer(entity);
            shootTimer.Run();
            if (shootTimer.Expired)
            {
                var target = detector.Detect(entity);
                if (target != null)
                {
                    Shoot(entity);
                }
                shootTimer.Frame = entity.RNG.Next(40, 45);
            }
        }
        public virtual Projectile Shoot(Entity entity)
        {
            entity.TriggerAnimation("Attack");

            var game = entity.Game;
            game.PlaySound(ShootSound, entity.Pos);

            Vector3 offset = ProjectileOffset;
            Vector3 velocity = ProjectileVelocity;
            if (entity.IsFacingLeft())
            {
                offset.x *= -1;
                velocity.x *= -1;
            }
            var projectile = game.Spawn(Projectile, entity.Pos + offset, entity).ToProjectile();
            projectile.Velocity = velocity;
            projectile.SetFaction(entity.GetFaction());
            return projectile;
        }
        public static FrameTimer GetShootTimer(Entity entity)
        {
            return entity.GetProperty<FrameTimer>("ShootTimer");
        }
        public static void SetShootTimer(Entity entity, FrameTimer timer)
        {
            entity.SetProperty("ShootTimer", timer);
        }
    }
}