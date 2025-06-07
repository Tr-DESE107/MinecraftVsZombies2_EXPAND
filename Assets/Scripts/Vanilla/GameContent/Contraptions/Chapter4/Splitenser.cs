using MVZ2.GameContent.Detections;
using MVZ2.GameContent.Projectiles;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Detections;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Properties;
using PVZEngine.Entities;
using PVZEngine.Level;
using Tools;

namespace MVZ2.GameContent.Contraptions
{
    [EntityBehaviourDefinition(VanillaContraptionNames.splitenser)]
    public class Splitenser : DispenserFamily
    {
        public Splitenser(string nsp, string name) : base(nsp, name)
        {
            detectorBack = new DispenserDetector()
            {
                ignoreHighEnemy = true,
                reversed = true,
            };
        }

        public override void Init(Entity entity)
        {
            base.Init(entity);
            InitShootTimer(entity);
            SetEvocationTimer(entity, new FrameTimer(120));
            SetRepeatTimer(entity, new FrameTimer(REPEAT_INTERVAL));
        }
        protected override void UpdateAI(Entity entity)
        {
            base.UpdateAI(entity);
            if (!entity.IsEvoked())
            {
                ShootTickSplit(entity);
                int repeatCount = GetRepeatCount(entity);
                if (repeatCount > 0)
                {
                    var repeatTimer = GetRepeatTimer(entity);
                    repeatTimer.Run(entity.GetAttackSpeed());
                    if (repeatTimer.Expired)
                    {
                        ShootBack(entity);
                        SetRepeatCount(entity, repeatCount - 1);
                        repeatTimer.Reset();
                    }
                }
                return;
            }

            EvokedUpdate(entity);
        }
        public void ShootTickSplit(Entity entity)
        {
            var shootTimer = GetShootTimer(entity);
            shootTimer.Run(entity.GetAttackSpeed());
            if (shootTimer.Expired)
            {
                var frontTarget = detector.Detect(entity);
                if (frontTarget != null)
                {
                    ShootFront(entity);
                }
                var backTarget = detectorBack.Detect(entity);
                if (backTarget != null)
                {
                    RepeatShootBack(entity);
                }
                shootTimer.ResetTime(GetTimerTime(entity));
            }
        }
        public Entity ShootFront(Entity entity)
        {
            entity.TriggerAnimation("ShootFront");
            return entity.ShootProjectile();
        }
        public Entity ShootBack(Entity entity)
        {
            entity.TriggerAnimation("ShootBack");

            var param = entity.GetShootParams();

            var offset = entity.GetShotOffset();
            offset.x *= -1;
            offset = entity.ModifyShotOffset(offset);
            param.position = entity.Position + offset;

            var vel = param.velocity;
            vel.x *= -1;
            param.velocity = vel;

            return entity.ShootProjectile(param);
        }
        public Entity ShootLargeArrowBack(Entity entity)
        {
            entity.TriggerAnimation("ShootBack");

            var param = entity.GetShootParams();

            var offset = entity.GetShotOffset();
            offset.x *= -1;
            offset = entity.ModifyShotOffset(offset);
            param.position = entity.Position + offset;

            var vel = param.velocity;
            vel.x *= -1;
            param.velocity = vel.normalized;

            param.projectileID = VanillaProjectileID.largeArrow;
            param.damage = entity.GetDamage() * 30;
            param.soundID = VanillaSoundID.spellCard;

            return entity.ShootProjectile(param);
        }
        public void RepeatShootBack(Entity entity)
        {
            SetRepeatCount(entity, 2);
            var repeatTimer = GetRepeatTimer(entity);
            repeatTimer.ResetTime(REPEAT_INTERVAL);
            repeatTimer.Frame = 0;
        }
        protected override void OnEvoke(Entity entity)
        {
            base.OnEvoke(entity);
            var evocationTimer = GetEvocationTimer(entity);
            evocationTimer.Reset();
            entity.SetEvoked(true);
        }
        public static FrameTimer GetEvocationTimer(Entity entity) => entity.GetBehaviourField<FrameTimer>(PROP_EVOCATION_TIMER);
        public static void SetEvocationTimer(Entity entity, FrameTimer timer) => entity.SetBehaviourField(PROP_EVOCATION_TIMER, timer);
        public static FrameTimer GetRepeatTimer(Entity entity) => entity.GetBehaviourField<FrameTimer>(PROP_REPEAT_TIMER);
        public static void SetRepeatTimer(Entity entity, FrameTimer timer) => entity.SetBehaviourField(PROP_REPEAT_TIMER, timer);
        public static int GetRepeatCount(Entity entity) => entity.GetBehaviourField<int>(PROP_REPEAT_COUNT);
        public static void SetRepeatCount(Entity entity, int timer) => entity.SetBehaviourField(PROP_REPEAT_COUNT, timer);
        private void EvokedUpdate(Entity entity)
        {
            var evocationTimer = GetEvocationTimer(entity);
            evocationTimer.Run();
            if (evocationTimer.PassedInterval(2))
            {
                var frontProjectile = ShootFront(entity);
                frontProjectile.Velocity *= 2;

                var backProjectile = ShootBack(entity);
                backProjectile.Velocity *= 2;
            }
            if (evocationTimer.Expired)
            {
                ShootLargeArrowBack(entity);
                entity.SetEvoked(false);
                var shootTimer = GetShootTimer(entity);
                shootTimer.Reset();
            }
        }
        private Detector detectorBack;
        public const int REPEAT_INTERVAL = 5;
        public static readonly VanillaEntityPropertyMeta<FrameTimer> PROP_EVOCATION_TIMER = new VanillaEntityPropertyMeta<FrameTimer>("EvocationTimer");
        public static readonly VanillaEntityPropertyMeta<FrameTimer> PROP_REPEAT_TIMER = new VanillaEntityPropertyMeta<FrameTimer>("RepeatTimer");
        public static readonly VanillaEntityPropertyMeta<int> PROP_REPEAT_COUNT = new VanillaEntityPropertyMeta<int>("RepeatCount");
    }
}
