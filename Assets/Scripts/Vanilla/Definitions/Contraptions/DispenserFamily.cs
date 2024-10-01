using MVZ2.Extensions;
using MVZ2.Vanilla;
using PVZEngine;
using PVZEngine.Definitions;
using PVZEngine.Level;
using Tools;
using UnityEngine;

namespace MVZ2.GameContent.Contraptions
{
    public abstract class DispenserFamily : VanillaContraption
    {
        public DispenserFamily(string nsp, string name) : base(nsp, name)
        {
            SetProperty(EntityProperties.DAMAGE, 20);
            SetProperty(BuiltinEntityProps.PLACE_SOUND, SoundID.stone);
            SetProperty(VanillaEntityProps.RANGE, -1);
            SetProperty(VanillaEntityProps.SHOT_VELOCITY, new Vector3(10, 0, 0));
            SetProperty(VanillaEntityProps.SHOT_OFFSET, new Vector3(25, 30, 0));
            SetProperty(VanillaEntityProps.SHOOT_SOUND, SoundID.shot);
            SetProperty(VanillaEntityProps.PROJECTILE_ID, ProjectileID.arrow);
            SetProperty(EntityProperties.SIZE, new Vector3(32, 48, 32));
            detector = new DispenserDetector()
            {
                ignoreHighEnemy = true
            };
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
            return entity.ShootProjectile();
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
        private Detector detector;

    }
}