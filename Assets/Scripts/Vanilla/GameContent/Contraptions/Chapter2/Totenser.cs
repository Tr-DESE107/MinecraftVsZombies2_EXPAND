﻿using MVZ2.GameContent.Detections;
using MVZ2.GameContent.Effects;
using MVZ2.GameContent.Projectiles;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Detections;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Properties;
using PVZEngine;
using PVZEngine.Entities;
using PVZEngine.Level;
using Tools;
using UnityEngine;

namespace MVZ2.GameContent.Contraptions
{
    [EntityBehaviourDefinition(VanillaContraptionNames.totenser)]
    public class Totenser : DispenserFamily
    {
        public Totenser(string nsp, string name) : base(nsp, name)
        {
            fireBreathDetector = new FireBreathDetector()
            {
                fireBreathID = VanillaEffectID.fireBreath
            };
        }

        public override void Init(Entity entity)
        {
            base.Init(entity);
            InitShootTimer(entity);
            SetFireDetectTimer(entity, new FrameTimer(FIRE_DETECT_INTERVAL));
        }

        // 核心修改：添加随机发射逻辑
        public override Entity Shoot(Entity entity)
        {
            if (entity.RNG.Next(4) == 0)
            {
                var param = entity.GetShootParams();
                // 将 "mvz2:purpleArrow" 拆分为命名空间和名称
                param.projectileID = new NamespaceID("mvz2", "Poisonball");
                param.damage *= 3;
                entity.TriggerAnimation("Shoot");
                return entity.ShootProjectile(param);
            }
            return base.Shoot(entity);
        }

        protected override void UpdateAI(Entity entity)
        {
            base.UpdateAI(entity);
            if (!entity.IsEvoked())
            {
                ShootTick(entity);
            }
            else
            {
                EvokedUpdate(entity);
            }
        }
        protected override void UpdateLogic(Entity entity)
        {
            base.UpdateLogic(entity);
            UpdateFireBreath(entity);
        }
        public override void Evoke(Entity entity)
        {
            base.Evoke(entity);
            entity.SetEvoked(true);
            entity.TriggerAnimation("Throw");
            SetEvocationTime(entity, 0);
        }
        private void UpdateFireBreath(Entity entity)
        {
            var timer = GetFireDetectTimer(entity);
            timer.Run();
            if (timer.Expired)
            {
                var target = fireBreathDetector.Detect(entity);
                if (target != null && !entity.IsAIFrozen())
                {
                    entity.State = VanillaEntityStates.TOTENSER_FIRE_BREATH;
                }
                else
                {
                    entity.State = VanillaEntityStates.IDLE;
                }
                timer.Reset();
            }
            if (entity.State == VanillaEntityStates.TOTENSER_FIRE_BREATH)
            {
                var fireBreath = GetFireBreath(entity);
                var position = entity.Position + Vector3.up * 5;
                if (fireBreath == null || !fireBreath.Exists())
                {
                    fireBreath = entity.Level.Spawn(VanillaEffectID.fireBreath, position, entity);
                    fireBreath.SetParent(entity);
                    SetFireBreath(entity, fireBreath);
                }
                fireBreath.SetDamage(entity.GetDamage() * 2 / 3);
                fireBreath.Position = position;
                fireBreath.SetFactionAndDirection(entity.GetFaction());
            }
            else
            {
                var fireBreath = GetFireBreath(entity);
                if (fireBreath != null)
                {
                    fireBreath.SetParent(null);
                    SetFireBreath(entity, null);
                }
            }
        }
        private void EvokedUpdate(Entity entity)
        {
            var evocationTime = GetEvocationTime(entity);
            evocationTime++;
            if (evocationTime == HATCH_OPEN_TIME || evocationTime == HATCH_CLOSE_TIME)
            {
                entity.PlaySound(VanillaSoundID.stoneHatch);
            }
            if (evocationTime == THROW_JAVELIN_TIME)
            {
                var shootParams = entity.GetShootParams();
                shootParams.position = entity.Position + new Vector3(80 * entity.GetFacingX(), 80);
                shootParams.velocity = entity.GetFacingDirection() * 33;
                shootParams.projectileID = VanillaProjectileID.poisonJavelin;
                shootParams.damage = 1800;
                shootParams.soundID = VanillaSoundID.fling;
                var javelin = entity.ShootProjectile(shootParams);
                entity.PlaySound(VanillaSoundID.poisonCast);
            }
            if (evocationTime >= MAX_EVOCATION_TIME)
            {
                evocationTime = 0;
                entity.SetEvoked(false);
            }
            SetEvocationTime(entity, evocationTime);
        }
        public static FrameTimer GetFireDetectTimer(Entity entity) => entity.GetBehaviourField<FrameTimer>(ID, PROP_FIRE_DETECT_TIMER);
        public static void SetFireDetectTimer(Entity entity, FrameTimer value) => entity.SetBehaviourField(ID, PROP_FIRE_DETECT_TIMER, value);
        public static int GetEvocationTime(Entity entity) => entity.GetBehaviourField<int>(ID, PROP_EVOCATION_TIME);
        public static void SetEvocationTime(Entity entity, int value) => entity.SetBehaviourField(ID, PROP_EVOCATION_TIME, value);
        public static Entity GetFireBreath(Entity entity)
        {
            var entityID = entity.GetBehaviourField<EntityID>(ID, PROP_FIRE_BREATH);
            if (entityID == null)
                return null;
            return entityID.GetEntity(entity.Level);
        }
        public static void SetFireBreath(Entity entity, Entity value)
        {
            entity.SetBehaviourField(ID, PROP_FIRE_BREATH, new EntityID(value));
        }
        private static readonly NamespaceID ID = VanillaContraptionID.totenser;
        public static readonly VanillaEntityPropertyMeta PROP_FIRE_DETECT_TIMER = new VanillaEntityPropertyMeta("FireDetectTimer");
        public static readonly VanillaEntityPropertyMeta PROP_EVOCATION_TIME = new VanillaEntityPropertyMeta("EvocationTime");
        public static readonly VanillaEntityPropertyMeta PROP_FIRE_BREATH = new VanillaEntityPropertyMeta("FireBreath");
        private Detector fireBreathDetector;
        public const int FIRE_DETECT_INTERVAL = 7;
        public const int THROW_JAVELIN_TIME = 30;
        public const int MAX_EVOCATION_TIME = 48;
        public const int HATCH_OPEN_TIME = 1;
        public const int HATCH_CLOSE_TIME = 45;
    }
}
