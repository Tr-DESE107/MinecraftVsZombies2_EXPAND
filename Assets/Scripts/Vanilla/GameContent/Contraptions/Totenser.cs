using MVZ2.GameContent.Detections;
using MVZ2.GameContent.Effects;
using MVZ2.GameContent.Projectiles;
using MVZ2.Vanilla;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Detections;
using MVZ2.Vanilla.Entities;
using PVZEngine.Entities;
using Tools;
using UnityEngine;
using UnityEngine.UIElements;

namespace MVZ2.GameContent.Contraptions
{
    [Definition(VanillaContraptionNames.totenser)]
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
        public static FrameTimer GetFireDetectTimer(Entity entity) => entity.GetProperty<FrameTimer>("FireDetectTimer");
        public static void SetFireDetectTimer(Entity entity, FrameTimer value) => entity.SetProperty("FireDetectTimer", value);
        public static int GetEvocationTime(Entity entity) => entity.GetProperty<int>("EvocationTime");
        public static void SetEvocationTime(Entity entity, int value) => entity.SetProperty("EvocationTime", value);
        public static Entity GetFireBreath(Entity entity)
        {
            var entityID = entity.GetProperty<EntityID>("FireBreath");
            if (entityID == null)
                return null;
            return entityID.GetEntity(entity.Level);
        }
        public static void SetFireBreath(Entity entity, Entity value)
        {
            entity.SetProperty("FireBreath", new EntityID(value));
        }
        private Detector fireBreathDetector;
        public const int FIRE_DETECT_INTERVAL = 7;
        public const int THROW_JAVELIN_TIME = 30;
        public const int MAX_EVOCATION_TIME = 48;
        public const int HATCH_OPEN_TIME = 1;
        public const int HATCH_CLOSE_TIME = 45;
    }
}
