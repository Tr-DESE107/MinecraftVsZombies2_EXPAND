using System.Linq;
using MVZ2.GameContent.Projectiles;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Contraptions;
using MVZ2.Vanilla.Detections;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Properties;
using PVZEngine;
using PVZEngine.Entities;
using PVZEngine.Level;
using PVZEngine.Modifiers;
using Tools;
using UnityEngine;

namespace MVZ2.GameContent.Contraptions
{
    [EntityBehaviourDefinition(VanillaContraptionNames.pistenser)]
    public class Pistenser : DispenserFamily
    {
        public Pistenser(string nsp, string name) : base(nsp, name)
        {
            AddModifier(new Vector3Modifier(VanillaEntityProps.SHOT_OFFSET, NumberOperator.Add, PROP_EXTEND_SHOOT_OFFSET));
            AddModifier(new Vector3Modifier(EngineEntityProps.SIZE, NumberOperator.Add, PROP_EXTEND_SHOOT_OFFSET));
            AddModifier(new BooleanModifier(VanillaContraptionProps.BLOCKS_JUMP, PROP_BLOCKS_JUMP));
            detector.ignoreHighEnemy = false;
        }

        public override void Init(Entity entity)
        {
            base.Init(entity);
            InitShootTimer(entity);
            SetExtendDetectTimer(entity, new FrameTimer(DETECT_INTERVAL));
            SetEvocationTimer(entity, new FrameTimer(30));
        }
        protected override void UpdateAI(Entity entity)
        {
            base.UpdateAI(entity);
            if (!entity.IsEvoked())
            {
                ExtendUpdate(entity);
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
            var extend = GetExtend(entity);
            entity.SetAnimationFloat("Extend", extend);
            entity.SetProperty(PROP_EXTEND_SHOOT_OFFSET, Vector3.up * extend);
            entity.SetProperty(PROP_BLOCKS_JUMP, extend > 0);
        }

        protected override void OnEvoke(Entity entity)
        {
            base.OnEvoke(entity);
            entity.SetEvoked(true);
            entity.State = VanillaEntityStates.PISTENSER_SEED;
            var evocationTimer = GetEvocationTimer(entity);
            evocationTimer.Reset();
            entity.PlaySound(VanillaSoundID.pistonIn);
            entity.SetAnimationBool("HatchOn", true);
        }
        private void ExtendUpdate(Entity pistenser)
        {
            // 检查目标。
            var detectTimer = GetExtendDetectTimer(pistenser);
            detectTimer.Run();
            if (detectTimer.Expired)
            {
                var collider = detector.DetectWithTheMost(pistenser, e => e.Entity.GetRelativeY());
                SetExtendTarget(pistenser, collider?.Entity);
                detectTimer.Reset();
            }

            var target = GetExtendTarget(pistenser);
            // 存在目标，并且目标的最底部大于活塞发射器的基础子弹高度，则延长。
            if (target != null && target.Position.y > pistenser.Position.y + BASE_SHOT_HEIGHT)
            {
                // 目标延长高度为目标的中心点减去活塞发射器的基础子弹高度。
                float targetExtend = target.GetCenter().y - BASE_SHOT_HEIGHT;
                var extend = GetExtend(pistenser);

                // 目标延长高度和当前高度的差值必须大于或等于10。
                if (Mathf.Abs(targetExtend - extend) >= 10)
                {
                    ExtendToTargetHeight(pistenser, targetExtend);
                }
            }
            else
            {
                // 没有目标，或者目标不高，收回
                ExtendToTargetHeight(pistenser, 0);
            }
        }

        private void ExtendToTargetHeight(Entity pistenser, float targetHeight)
        {
            // 切换伸缩方向并播放音效。
            var direction = GetExtendDirection(pistenser);
            float height = GetExtend(pistenser);
            if (targetHeight > height)
            {
                if (direction != 1)
                {
                    pistenser.PlaySound(VanillaSoundID.pistonOut);
                    direction = 1;
                }
            }
            else if (targetHeight < height)
            {
                if (direction != -1)
                {
                    pistenser.PlaySound(VanillaSoundID.pistonIn);
                    direction = -1;
                }
            }
            else
            {
                direction = 0;
            }
            SetExtendDirection(pistenser, direction);

            // 伸缩。
            // 如果这次伸缩越过了目标高度，直接把高度修改为目标高度。
            float nextHeight = height + EXTEND_SPEED * direction;
            if ((targetHeight - nextHeight) * direction < 0)
            {
                height = targetHeight;
            }
            else
            {
                height = nextHeight;
            }

            height = Mathf.Max(0, height);
            SetExtend(pistenser, height);
        }

        private void EvokedUpdate(Entity entity)
        {
            FrameTimer evocationTimer = GetEvocationTimer(entity);
            evocationTimer.Run();
            if (entity.State == VanillaEntityStates.PISTENSER_SEED)
            {
                if (evocationTimer.Frame <= 15)
                {
                    SeedSpikeBalls(entity);
                    entity.State = VanillaEntityStates.IDLE;
                }
            }
            else
            {
                if (evocationTimer.Expired)
                {
                    entity.SetAnimationBool("HatchOn", false);
                    entity.SetEvoked(false);
                    entity.PlaySound(VanillaSoundID.pistonOut);
                }
            }
        }

        private void SeedSpikeBalls(Entity entity)
        {
            bool soundPlayed = false;
            var extend = GetExtend(entity);
            Vector3 spikeBallPos = entity.Position + (extend + 44) * Vector3.up;

            var projectileID = VanillaProjectileID.spikeBall;
            var projectileDefinition = entity.Level.Content.GetEntityDefinition(projectileID);
            var projectileGravity = projectileDefinition?.GetGravity() ?? 0;
            var targets = entity.Level.FindEntities(e => IsEvocationTarget(entity, e)).OrderByDescending(e => e.GetRelativeY()).Take(MAX_EVOCATION_TARGET);
            foreach (var target in targets)
            {
                if (!soundPlayed)
                {
                    entity.PlaySound(VanillaSoundID.smallExplosion);
                    soundPlayed = true;
                }

                var targetPos = target.Position;
                targetPos.y = target.GetGroundY();

                var shotParams = entity.GetShootParams();
                shotParams.position = spikeBallPos;
                shotParams.soundID = null;
                shotParams.projectileID = projectileID;
                shotParams.velocity = VanillaProjectileExt.GetLobVelocityByTime(spikeBallPos, targetPos, 24, projectileGravity);
                shotParams.damage = entity.GetDamage() * 9;
                entity.ShootProjectile(shotParams);
            }
        }
        private static bool IsEvocationTarget(Entity self, Entity target)
        {
            if (target == null)
                return false;
            if (target.IsDead)
                return false;
            if (!target.IsVulnerableEntity())
                return false;
            if (!self.IsHostile(target))
                return false;
            if (!Detection.CanDetect(target))
                return false;
            return true;
        }
        public static Entity GetExtendTarget(Entity entity)
        {
            var id = entity.GetBehaviourField<EntityID>(ID, PROP_EXTEND_TARGET);
            if (id == null)
                return null;
            return id.GetEntity(entity.Level);
        }
        public static void SetExtendTarget(Entity entity, Entity value)
        {
            entity.SetBehaviourField(ID, PROP_EXTEND_TARGET, new EntityID(value));
        }
        public static FrameTimer GetExtendDetectTimer(Entity entity) => entity.GetBehaviourField<FrameTimer>(ID, PROP_EXTEND_DETECT_TIMER);
        public static void SetExtendDetectTimer(Entity entity, FrameTimer value) => entity.SetBehaviourField(ID, PROP_EXTEND_DETECT_TIMER, value);
        public static FrameTimer GetEvocationTimer(Entity entity) => entity.GetBehaviourField<FrameTimer>(ID, PROP_EVOCATION_TIMER);
        public static void SetEvocationTimer(Entity entity, FrameTimer value) => entity.SetBehaviourField(ID, PROP_EVOCATION_TIMER, value);
        public static float GetExtend(Entity entity) => entity.GetBehaviourField<float>(ID, PROP_EXTEND);
        public static void SetExtend(Entity entity, float value) => entity.SetBehaviourField(ID, PROP_EXTEND, value);
        public static int GetExtendDirection(Entity entity) => entity.GetBehaviourField<int>(ID, PROP_EXTEND_DIRECTION);
        public static void SetExtendDirection(Entity entity, int value) => entity.SetBehaviourField(ID, PROP_EXTEND_DIRECTION, value);
        private static readonly NamespaceID ID = VanillaContraptionID.pistenser;
        public static readonly VanillaEntityPropertyMeta PROP_EXTEND = new VanillaEntityPropertyMeta("Extend");
        public static readonly VanillaEntityPropertyMeta PROP_EXTEND_SHOOT_OFFSET = new VanillaEntityPropertyMeta("ExtendShootOffset");
        public static readonly VanillaEntityPropertyMeta PROP_BLOCKS_JUMP = new VanillaEntityPropertyMeta("BlocksJump");
        public static readonly VanillaEntityPropertyMeta PROP_EXTEND_DIRECTION = new VanillaEntityPropertyMeta("ExtendDirection");
        public static readonly VanillaEntityPropertyMeta PROP_EVOCATION_TIMER = new VanillaEntityPropertyMeta("EvocationTimer");
        public static readonly VanillaEntityPropertyMeta PROP_EXTEND_DETECT_TIMER = new VanillaEntityPropertyMeta("ExtendDetectTimer");
        public static readonly VanillaEntityPropertyMeta PROP_EXTEND_TARGET = new VanillaEntityPropertyMeta("ExtendTarget");
        public const float BASE_SHOT_HEIGHT = 30;
        public const float EXTEND_SPEED = 10;
        public const int DETECT_INTERVAL = 8;
        public const int MAX_EVOCATION_TARGET = 10;
    }
}
