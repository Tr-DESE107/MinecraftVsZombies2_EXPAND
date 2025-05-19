using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Properties;
using PVZEngine.Damages;
using PVZEngine.Entities;
using PVZEngine.Level;
using Tools;
using UnityEngine;

namespace MVZ2.GameContent.Effects
{
    [EntityBehaviourDefinition(VanillaEffectNames.zombieBlock)]
    public class ZombieBlock : EffectBehaviour
    {

        #region 公有方法
        public ZombieBlock(string nsp, string name) : base(nsp, name)
        {
        }
        #endregion
        public override void Init(Entity entity)
        {
            base.Init(entity);
            SetMoveCooldownTimer(entity, new FrameTimer(0));
            entity.CollisionMaskHostile |= EntityCollisionHelper.MASK_VULNERABLE;
        }
        public override void Update(Entity entity)
        {
            base.Update(entity);
            var parent = entity.Parent;
            if (!parent.ExistsAndAlive())
            {
                entity.Remove();
                return;
            }
            var cooldownTimer = GetMoveCooldownTimer(entity);
            cooldownTimer.Run();
            if (cooldownTimer.Expired)
            {
                var targetPosition = GetTargetPosition(entity);
                if (!IsReached(entity))
                {
                    var vel = (targetPosition - entity.Position).normalized * MOVE_SPEED;
                    entity.Velocity = vel;
                }
                else
                {
                    entity.Velocity = Vector3.zero;
                    entity.Position = entity.Position * 0.7f + targetPosition * 0.3f;
                }
            }
            else
            {
                var startPosition = GetStartPosition(entity);
                entity.Position = entity.Position * 0.7f + startPosition * 0.3f;
            }
        }
        public override void PostCollision(EntityCollision collision, int state)
        {
            base.PostCollision(collision, state);
            if (state != EntityCollisionHelper.STATE_ENTER)
                return;
            var block = collision.Entity;
            var other = collision.Other;
            if (!block.IsHostile(other))
                return;
            collision.OtherCollider.TakeDamage(block.GetDamage(), new DamageEffectList(), block);
        }
        public static bool IsReached(Entity entity)
        {
            var targetPos = GetTargetPosition(entity);
            return (targetPos - entity.Position).sqrMagnitude <= MOVE_SPEED * MOVE_SPEED;
        }
        public static void SetMoveCooldown(Entity entity, int time)
        {
            var moveTimer = GetMoveCooldownTimer(entity);
            if (moveTimer == null)
                return;
            moveTimer.ResetTime(time);
        }
        public static FrameTimer GetMoveCooldownTimer(Entity entity) => entity.GetBehaviourField<FrameTimer>(PROP_MOVE_COOLDOWN_TIMER);
        public static void SetMoveCooldownTimer(Entity entity, FrameTimer value) => entity.SetBehaviourField(PROP_MOVE_COOLDOWN_TIMER, value);
        public static Vector3 GetStartPosition(Entity entity) => entity.GetBehaviourField<Vector3>(PROP_START_POSITION);
        public static void SetStartPosition(Entity entity, Vector3 value) => entity.SetBehaviourField(PROP_START_POSITION, value);
        public static void SetStartGrid(Entity entity, int column, int lane) => SetStartPosition(entity, entity.Level.GetEntityGridPosition(column, lane));
        public static Vector3 GetTargetPosition(Entity entity) => entity.GetBehaviourField<Vector3>(PROP_TARGET_POSITION);
        public static void SetTargetPosition(Entity entity, Vector3 value) => entity.SetBehaviourField(PROP_TARGET_POSITION, value);
        public static void SetTargetGrid(Entity entity, int column, int lane) => entity.SetBehaviourField(PROP_TARGET_POSITION, entity.Level.GetEntityGridPosition(column, lane));

        public const float MOVE_SPEED = 20;
        private static readonly VanillaEntityPropertyMeta PROP_MOVE_COOLDOWN_TIMER = new VanillaEntityPropertyMeta("MoveCooldownTimer");
        private static readonly VanillaEntityPropertyMeta PROP_START_POSITION = new VanillaEntityPropertyMeta("StartPosition");
        private static readonly VanillaEntityPropertyMeta PROP_TARGET_POSITION = new VanillaEntityPropertyMeta("TargetPosition");
    }
}