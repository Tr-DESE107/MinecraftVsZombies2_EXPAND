using MVZ2.GameContent.Detections;
using MVZ2.Vanilla.Detections;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Level;
using MVZ2.Vanilla.Properties;
using PVZEngine.Entities;
using PVZEngine.Level;
using Tools;
using UnityEngine;

namespace MVZ2.GameContent.Contraptions
{
    [EntityBehaviourDefinition(VanillaContraptionNames.beacon)]
    public class Beacon : ContraptionBehaviour
    {
        public Beacon(string nsp, string name) : base(nsp, name)
        {
            detector = new BeaconDetector();
        }

        public override void Init(Entity entity)
        {
            base.Init(entity);
            var shootTimer = new FrameTimer(GetTimerTime(entity));
            SetShootTimer(entity, shootTimer);
        }

        protected override void UpdateAI(Entity entity)
        {
            base.UpdateAI(entity);
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
        public void OnShootTick(Entity entity)
        {
            var shotSpeed = entity.GetShotVelocity().magnitude;
            foreach (var direction in shootDirections)
            {
                var shootParams = entity.GetShootParams();
                shootParams.velocity = direction * shotSpeed;
                entity.ShootProjectile(shootParams);
            }
            entity.TriggerAnimation("Shoot");
        }
        public static FrameTimer GetShootTimer(Entity entity) => entity.GetBehaviourField<FrameTimer>(PROP_SHOOT_TIMER);
        public static void SetShootTimer(Entity entity, FrameTimer timer) => entity.SetBehaviourField(PROP_SHOOT_TIMER, timer);
        protected virtual int GetTimerTime(Entity entity)
        {
            if (entity.Level.IsIZombie())
            {
                return ATTACK_INTERVAL_MAX;
            }
            return entity.RNG.Next(ATTACK_INTERVAL_MIN, ATTACK_INTERVAL_MAX + 1);
        }
        protected Detector detector;
        private const int ATTACK_INTERVAL_MIN = 40;
        private const int ATTACK_INTERVAL_MAX = 45;
        public static Vector3[] shootDirections = new Vector3[]
        {
            new Vector3(-1, 0, 0), // Back
            new Vector3(0, 0, 1), // Up
            new Vector3(0, 0, -1), // Down
            new Vector3(0.866025f, 0, 0.5f).normalized, // Front-Up
            new Vector3(0.866025f, 0, -0.5f).normalized, // Front-Down
        };
        public static readonly VanillaEntityPropertyMeta<FrameTimer> PROP_SHOOT_TIMER = new VanillaEntityPropertyMeta<FrameTimer>("shoot_timer");
    }
}