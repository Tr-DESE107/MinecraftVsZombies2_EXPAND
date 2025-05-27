using MVZ2.GameContent.Detections;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Properties;
using PVZEngine;
using PVZEngine.Entities;
using Tools;
using UnityEngine;

namespace MVZ2.GameContent.Contraptions
{
    public abstract class DispenserFamily : ContraptionBehaviour
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
            shootTimer.Run(entity.GetAttackSpeed());
            if (shootTimer.Expired)
            {
                var target = detector.Detect(entity);
                if (target != null)
                {
                    OnShootTick(entity);
                }
                shootTimer.ResetTime(GetTimerTime(entity));
            }
        }
        public virtual void OnShootTick(Entity entity)
        {
            Shoot(entity);
        }
        public virtual Entity Shoot(Entity entity)
        {
            entity.TriggerAnimation("Shoot");
            return entity.ShootProjectile();
        }
        public static FrameTimer GetShootTimer(Entity entity) => entity.GetBehaviourField<FrameTimer>(PROP_SHOOT_TIMER);
        public static void SetShootTimer(Entity entity, FrameTimer timer) => entity.SetBehaviourField(PROP_SHOOT_TIMER, timer);
        protected virtual int GetTimerTime(Entity entity)
        {
            return Mathf.FloorToInt(entity.RNG.Next(40, 45));
        }
        protected DispenserDetector detector;
        private const string PROP_REGION = "dispenser_family";
        [EntityPropertyRegistry(PROP_REGION)]
        public static readonly VanillaEntityPropertyMeta<FrameTimer> PROP_SHOOT_TIMER = new VanillaEntityPropertyMeta<FrameTimer>("ShootTimer");
    }
}