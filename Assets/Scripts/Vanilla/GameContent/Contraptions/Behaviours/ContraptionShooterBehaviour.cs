﻿using MVZ2.GameContent.Detections;
using MVZ2.Vanilla.Contraptions;
using MVZ2.Vanilla.Detections;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Level;
using MVZ2.Vanilla.Properties;
using PVZEngine;
using PVZEngine.Entities;
using Tools;

namespace MVZ2.GameContent.Contraptions
{
    public abstract class ContraptionShooterBehaviour : AIEntityBehaviour
    {
        public ContraptionShooterBehaviour(string nsp, string name) : base(nsp, name)
        {
            detector = GetDetector();
        }
        public override void Init(Entity entity)
        {
            base.Init(entity);
            SetShootTimer(entity, new FrameTimer(GetTimerTime(entity)));
        }
        protected override void UpdateAI(Entity entity)
        {
            base.UpdateAI(entity);
            if (!entity.IsEvoked())
            {
                Tick(entity);
            }
        }
        public void Tick(Entity entity)
        {
            var shootTimer = GetShootTimer(entity);
            shootTimer.Run(entity.GetAttackSpeed());
            if (shootTimer.Expired)
            {
                var target = detector.Detect(entity);
                if (target != null)
                {
                    OnTickEnd(entity);
                }
                shootTimer.ResetTime(GetTimerTime(entity));
            }
        }
        public virtual void OnTickEnd(Entity entity)
        {
            Shoot(entity);
        }
        public virtual Entity Shoot(Entity entity)
        {
            entity.TriggerAnimation("Shoot");
            return entity.ShootProjectile();
        }
        protected virtual int GetTimerTime(Entity entity)
        {
            if (entity.Level.IsIZombie())
            {
                return ATTACK_INTERVAL_MAX;
            }
            return entity.RNG.Next(ATTACK_INTERVAL_MIN, ATTACK_INTERVAL_MAX + 1);
        }
        protected virtual Detector GetDetector()
        {
            return new DispenserDetector()
            {
                ignoreHighEnemy = true
            };
        }
        public static FrameTimer GetShootTimer(Entity entity) => entity.GetBehaviourField<FrameTimer>(PROP_SHOOT_TIMER);
        public static void SetShootTimer(Entity entity, FrameTimer timer) => entity.SetBehaviourField(PROP_SHOOT_TIMER, timer);
        public const string PROP_REGION = "contraption_shooter";
        [PropertyRegistry(PROP_REGION)]
        public static readonly VanillaEntityPropertyMeta<FrameTimer> PROP_SHOOT_TIMER = new VanillaEntityPropertyMeta<FrameTimer>("ShootTimer");
        protected Detector detector;
        private const int ATTACK_INTERVAL_MIN = 40;
        private const int ATTACK_INTERVAL_MAX = 45;
    }
}
