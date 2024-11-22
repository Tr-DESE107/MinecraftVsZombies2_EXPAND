using MVZ2.GameContent.Detections;
using MVZ2.Vanilla.Detections;
using MVZ2.Vanilla.Entities;
using PVZEngine.Entities;
using Tools;
using UnityEngine;

namespace MVZ2.GameContent.Contraptions
{
    public abstract class DispenserFamily : VanillaContraption
    {
        public DispenserFamily(string nsp, string name) : base(nsp, name)
        {
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