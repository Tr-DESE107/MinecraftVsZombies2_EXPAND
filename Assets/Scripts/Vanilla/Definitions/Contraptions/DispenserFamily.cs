using MVZ2.Vanilla;
using PVZEngine;
using PVZEngine.Definitions;
using PVZEngine.LevelManagement;
using Tools;
using UnityEngine;

namespace MVZ2.GameContent.Contraptions
{
    public abstract class DispenserFamily : VanillaContraption
    {
        public DispenserFamily(string nsp, string name) : base(nsp, name)
        {
            SetProperty(EntityProperties.DAMAGE, 20);
            SetProperty(EntityProps.PLACE_SOUND, SoundID.stone);
            detector = new DispenserDetector()
            {
                ignoreHighEnemy = true,
                range = Range,
                shootOffset = ProjectileOffset,
                projectileID = Entity,
            };
            SetProperty(EntityProperties.SIZE, new Vector3(32, 48, 32));
        }

        public void InitShootTimer(Entity entity)
        {
            var shootTimer = new FrameTimer(GetTimerTime(entity));
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
                shootTimer.MaxFrame = GetTimerTime(entity);
                shootTimer.Reset();
            }
        }
        public virtual Entity Shoot(Entity entity)
        {
            entity.TriggerAnimation("Shoot");

            var game = entity.Level;
            game.PlaySound(ShootSound, entity.Pos);

            Vector3 offset = ProjectileOffset;
            Vector3 velocity = ProjectileVelocity;
            if (entity.IsFacingLeft())
            {
                offset.x *= -1;
                velocity.x *= -1;
            }
            velocity = ModifyProjectileVelocity(entity, velocity);

            var projectile = game.Spawn(Entity, entity.Pos + offset, entity);
            projectile.SetDamage(entity.GetDamage());
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
        protected virtual int GetTimerTime(Entity entity)
        {
            return Mathf.FloorToInt(entity.RNG.Next(40, 45) * entity.GetAttackSpeed());
        }
        protected virtual Vector3 ProjectileOffset => projectileOffset;
        protected virtual Vector3 ProjectileVelocity => projectileVelocity;
        protected virtual float Range => -1;
        protected virtual NamespaceID ShootSound => SoundID.shot;
        protected virtual NamespaceID Entity => ProjectileID.arrow;
        private Vector3 projectileOffset = new Vector3(25, 30, 0);
        private Vector3 projectileVelocity = new Vector3(10, 0, 0);
        private Detector detector;

    }
}