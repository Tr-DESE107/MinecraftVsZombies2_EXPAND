using System.Collections.Generic;
using MVZ2.GameContent.Detections;
using MVZ2.GameContent.Effects;
using MVZ2.Vanilla;
using MVZ2.Vanilla.Detections;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Level;
using PVZEngine;
using PVZEngine.Damages;
using PVZEngine.Entities;
using Tools;
using UnityEngine;

namespace MVZ2.GameContent.Contraptions
{
    [Definition(VanillaContraptionNames.spikeBlock)]
    public class SpikeBlock : ContraptionBehaviour
    {
        public SpikeBlock(string nsp, string name) : base(nsp, name)
        {
            detector = new SpikeBlockDetector();
            detectorEvoked = new SpikeBlockDetector(true);
        }
        public override void Init(Entity entity)
        {
            base.Init(entity);
            SetAttackTimer(entity, new FrameTimer(ATTACK_TIME));
            SetEvocationTimer(entity, new FrameTimer(EVOCATION_DURATION));
            SetEvocationAttackTimer(entity, new FrameTimer(EVOCATION_INTERVAL));
        }
        protected override void UpdateAI(Entity entity)
        {
            base.UpdateAI(entity);
            if (!entity.IsEvoked())
            {
                var timer = GetAttackTimer(entity);
                timer.Run(entity.GetAttackSpeed());
                if (timer.Expired)
                {
                    timer.Reset();
                    detectBuffer.Clear();
                    detector.DetectMultiple(entity, detectBuffer);
                    if (detectBuffer.Count > 0)
                    {
                        foreach (var target in detectBuffer)
                        {
                            target.TakeDamage(entity.GetDamage(), new DamageEffectList(), entity);
                        }
                        entity.TriggerAnimation("Attack");
                    }
                }
            }
            else
            {
                var evocationTimer = GetEvocationTimer(entity);
                var attackTimer = GetEvocationAttackTimer(entity);
                evocationTimer.Run();
                attackTimer.Run(entity.GetAttackSpeed());
                detectBuffer.Clear();
                detectorEvoked.DetectMultiple(entity, detectBuffer);

                // 造成伤害
                if (attackTimer.Expired)
                {
                    attackTimer.Reset();
                    foreach (var target in detectBuffer)
                    {
                        target.TakeDamage(entity.GetDamage(), new DamageEffectList(), entity);
                    }
                    entity.TriggerAnimation("Attack");
                }
                // 拉近敌人
                foreach (var target in detectBuffer)
                {
                    var targetEnt = target.Entity;
                    if (targetEnt.Type == EntityTypes.ENEMY)
                    {
                        targetEnt.Position += (entity.Position - targetEnt.Position).normalized * PULL_SPEED;
                    }
                }
                // 产生特效
                var rng = entity.RNG;
                var level = entity.Level;
                var z = entity.Position.z;
                var padding = level.GetGridWidth() * 0.25f;
                var minX = level.GetGridLeftX() + padding;
                var maxX = level.GetGridRightX() - padding;
                for (int i = 0; i < 5; i++)
                {
                    var x = rng.Next(minX, maxX);
                    var y = level.GetGroundY(x, z);
                    var pos = new Vector3(x, y, z);
                    entity.Spawn(VanillaEffectID.spikeParticles, pos);
                }
                // 结束
                if (evocationTimer.Expired)
                {
                    entity.SetEvoked(false);
                }
            }
        }
        protected override void OnEvoke(Entity entity)
        {
            base.OnEvoke(entity);
            entity.SetEvoked(true);
            var evocationTimer = GetEvocationTimer(entity);
            evocationTimer.ResetTime(EVOCATION_DURATION);
            var attackTimer = GetEvocationAttackTimer(entity);
            attackTimer.ResetTime(EVOCATION_INTERVAL);
        }
        public static FrameTimer GetAttackTimer(Entity entity) => entity.GetBehaviourField<FrameTimer>(ID, PROP_ATTACK_TIMER);
        public static void SetAttackTimer(Entity entity, FrameTimer timer) => entity.SetBehaviourField(ID, PROP_ATTACK_TIMER, timer);
        public static FrameTimer GetEvocationTimer(Entity entity) => entity.GetBehaviourField<FrameTimer>(ID, PROP_EVOCATION_TIMER);
        public static void SetEvocationTimer(Entity entity, FrameTimer timer) => entity.SetBehaviourField(ID, PROP_EVOCATION_TIMER, timer);
        public static FrameTimer GetEvocationAttackTimer(Entity entity) => entity.GetBehaviourField<FrameTimer>(ID, PROP_EVOCATION_ATTACK_TIMER);
        public static void SetEvocationAttackTimer(Entity entity, FrameTimer timer) => entity.SetBehaviourField(ID, PROP_EVOCATION_ATTACK_TIMER, timer);
        public const int ATTACK_TIME = 30;
        public const int EVOCATION_DURATION = 120;
        public const int EVOCATION_INTERVAL = 4;
        public const float PULL_SPEED = 10;
        public const string PROP_ATTACK_TIMER = "AttackTimer";
        public const string PROP_EVOCATION_TIMER = "EvocationTimer";
        public const string PROP_EVOCATION_ATTACK_TIMER = "EvocationAttackTimer";
        public static readonly NamespaceID ID = VanillaContraptionID.spikeBlock;
        private Detector detector;
        private Detector detectorEvoked;
        private List<EntityCollider> detectBuffer = new List<EntityCollider>();
    }
}
