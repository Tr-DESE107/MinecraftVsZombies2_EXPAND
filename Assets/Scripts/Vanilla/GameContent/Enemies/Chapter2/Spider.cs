using MVZ2.GameContent.Buffs.Enemies;
using MVZ2.Vanilla.Contraptions;
using MVZ2.Vanilla.Detections;
using MVZ2.Vanilla.Enemies;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Properties;
using PVZEngine;
using PVZEngine.Entities;
using PVZEngine.Level;
using UnityEngine;

namespace MVZ2.GameContent.Enemies
{
    [EntityBehaviourDefinition(VanillaEnemyNames.spider)]
    public class Spider : MeleeEnemy
    {
        public Spider(string nsp, string name) : base(nsp, name)
        {
        }
        protected override void UpdateAI(Entity entity)
        {
            base.UpdateAI(entity);

            if (entity.State == VanillaEntityStates.SPIDER_CLIMB)
            {
                var climbTarget = GetClimbTarget(entity);
                if (climbTarget != null && climbTarget.Exists())
                {
                    // 正在垂直攀爬，修改位置。
                    var peak = GetClimbTargetPeak(climbTarget);
                    if (entity.Position.y < peak)
                    {
                        var position = entity.Position;
                        position.y = Mathf.Min(position.y + (0.83f * entity.GetSpeed()), peak);
                        entity.Position = position;
                    }
                }
            }
        }
        protected override void UpdateLogic(Entity entity)
        {
            base.UpdateLogic(entity);

            // 攀爬目标不合适，那就取消。
            var climbTarget = GetClimbTarget(entity);
            if (!ValidateClimbTarget(entity, climbTarget))
            {
                climbTarget = null;
                SetClimbTarget(entity, null);
            }
            if (climbTarget != null && climbTarget.Exists())
            {
                // 攀爬目标合适。
                // 没有攀爬BUFF，那就加上一个。
                if (!entity.HasBuff<SpiderClimbBuff>())
                {
                    entity.AddBuff<SpiderClimbBuff>();
                }

                if (entity.State == VanillaEntityStates.SPIDER_CLIMB)
                {
                    // 正在攀爬目标上行走。
                    var velocity = entity.Velocity;
                    velocity.y = Mathf.Max(0, velocity.y);
                    entity.Velocity = velocity;
                }
            }
            else
            {
                // 没有正在攀爬，移除增益。
                if (entity.HasBuff<SpiderClimbBuff>())
                {
                    entity.RemoveBuffs<SpiderClimbBuff>();
                }
            }
            // 设置血量状态。
            entity.SetAnimationInt("HealthState", entity.GetHealthState(2));
        }
        public override void PostCollision(EntityCollision collision, int state)
        {
            if (collision.Collider.IsMain() && collision.OtherCollider.IsMain())
            {
                var enemy = collision.Entity;
                var other = collision.Other;
                if (state != EntityCollisionHelper.STATE_EXIT)
                {
                    // 可以成为攀爬目标。
                    if (CanClimb(enemy, other))
                    {
                        // 如果目标可以爬，并且两者之间的关系处在可以爬的状态，那就设置为攀爬目标
                        if (!ValidateClimbTarget(enemy, GetClimbTarget(enemy)) && ValidateClimbTarget(enemy, other))
                        {
                            SetClimbTarget(enemy, other);
                        }

                        // 只要能爬就不会进行攻击。
                        return;
                    }
                }
                else
                {
                    // 离开碰撞时，如果对面就是攀爬目标，取消攀爬目标。
                    if (GetClimbTarget(enemy) == other)
                    {
                        SetClimbTarget(enemy, null);
                    }
                }
            }
            base.PostCollision(collision, state);
        }
        protected override int GetActionState(Entity enemy)
        {
            var state = base.GetActionState(enemy);
            if (state == VanillaEntityStates.WALK || state == VanillaEntityStates.ATTACK)
            {
                if (IsClimbingVertically(enemy))
                {
                    return VanillaEntityStates.SPIDER_CLIMB;
                }
            }
            return state;
        }
        public static bool IsClimbingVertically(Entity spider)
        {
            var target = GetClimbTarget(spider);
            if (target == null)
                return false;

            return spider.Position.y < GetClimbTargetPeak(target);
        }
        public static float GetClimbTargetPeak(Entity target)
        {
            return target.GetBounds().max.y - 5;
        }
        protected virtual bool CanClimb(Entity enemy, Entity target)
        {
            if (target == null || !target.Exists() || target.IsDead)
                return false;
            if (!enemy.IsHostile(target))
                return false;
            if (!Detection.IsInSameRow(enemy, target))
                return false;
            if (!Detection.CanDetect(target))
                return false;
            if (target.Type != EntityTypes.PLANT)
                return false;
            if (target.IsFloor() || !target.IsDefensive())
                return false;
            return true;
        }
        protected virtual bool ValidateClimbTarget(Entity enemy, Entity target)
        {
            return CanClimb(enemy, target) && !enemy.IsAIFrozen() && !enemy.IsDead && Detection.IsInFrontOf(enemy, target, -enemy.GetScaledSize().x * 0.25f);
        }

        public static Entity GetClimbTarget(Entity spider)
        {
            var id = spider.GetBehaviourField<EntityID>(ID, PROP_CLIMB_TARGET_ID);
            if (id == null)
                return null;
            return id.GetEntity(spider.Level);
        }
        public static void SetClimbTarget(Entity spider, Entity value)
        {
            spider.SetBehaviourField(ID, PROP_CLIMB_TARGET_ID, new EntityID(value));
        }
        public static readonly NamespaceID ID = VanillaEnemyID.spider;
        public static readonly VanillaEntityPropertyMeta PROP_CLIMB_TARGET_ID = new VanillaEntityPropertyMeta("ClimbTargetID");
    }
}
