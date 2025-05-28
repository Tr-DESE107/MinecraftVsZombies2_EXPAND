﻿using System.Collections.Generic;
using System.Linq;
using MVZ2.GameContent.Damages;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Properties;
using MVZ2Logic.Level;
using PVZEngine;
using PVZEngine.Callbacks;
using PVZEngine.Damages;
using PVZEngine.Entities;
using PVZEngine.Level;

namespace MVZ2.GameContent.Bosses
{
    [EntityBehaviourDefinition(VanillaBossNames.theGiantSnakeTail)]
    public partial class TheGiantSnakeTail : BossBehaviour
    {
        public TheGiantSnakeTail(string nsp, string name) : base(nsp, name)
        {
        }

        #region 回调
        public override void Init(Entity boss)
        {
            base.Init(boss);
            boss.CollisionMaskHostile |=
                EntityCollisionHelper.MASK_PLANT |
                EntityCollisionHelper.MASK_ENEMY |
                EntityCollisionHelper.MASK_OBSTACLE |
                EntityCollisionHelper.MASK_BOSS;
            boss.CollisionMaskFriendly |= EntityCollisionHelper.MASK_BOSS;
            SetTargetGridIndex(boss, boss.GetGridIndex());
        }
        protected override void UpdateLogic(Entity entity)
        {
            base.UpdateLogic(entity);
            var parent = entity.Parent;
            if (parent == null || !parent.Exists())
            {
                entity.Remove();
                return;
            }
        }
        public override void PostCollision(EntityCollision collision, int state)
        {
            base.PostCollision(collision, state);
            var other = collision.Other;
            var self = collision.Entity;
            if (self.IsDead)
                return;
            if (!other.Exists())
                return;
            if (other.IsEntityOf(VanillaBossID.theGiant) && TheGiant.IsSnake(other) && (other.GetCenter() - self.GetCenter()).magnitude < KILL_SNAKE_DISTANCE)
            {
                other.TakeDamage(COLLIDE_SELF_DAMAGE, new DamageEffectList(VanillaDamageEffects.MUTE), self);
                TheGiant.KillSnake(other);
                return;
            }
            if (other.IsHostile(self))
            {
                var damageEffects = new DamageEffectList(VanillaDamageEffects.DAMAGE_BODY_AFTER_ARMOR_BROKEN, VanillaDamageEffects.MUTE);
                collision.OtherCollider.TakeDamage(self.GetDamage(), damageEffects, self);
            }
        }
        public override void PreTakeDamage(DamageInput damageInfo, CallbackResult result)
        {
            base.PreTakeDamage(damageInfo, result);
            var self = damageInfo.Entity;
            var parent = self.Parent;
            if (parent != null && parent.Exists())
            {
                var oldEffects = damageInfo.Effects.GetEffects();
                var newEffects = oldEffects.Union(transferDamageExtraEffects);
                var effects = new DamageEffectList(newEffects.ToArray());
                parent.TakeDamage(damageInfo.Amount, effects, damageInfo.Source, damageInfo.ShieldTarget);
                self.DamageBlink();
                result.SetFinalValue(false);
            }
        }
        public static void PassTargetGrids(Entity parent, int gridIndex)
        {
            var childID = GetChildTail(parent);
            var child = childID?.GetEntity(parent.Level);
            if (child != null)
            {
                var targetGridIndex = GetTargetGridIndex(child);
                PassTargetGrids(child, targetGridIndex);
                SetTargetGridIndex(child, gridIndex);
            }
        }
        public static void MoveTail(Entity parent, float speed)
        {
            var childID = GetChildTail(parent);
            var child = childID?.GetEntity(parent.Level);
            if (child != null)
            {
                MoveTail(child, speed);

                var targetGridIndex = GetTargetGridIndex(child);
                if (targetGridIndex >= 0)
                {
                    child.MoveOrthogonally(targetGridIndex, speed);
                }
            }
        }
        public static Entity FindTail(Entity parent)
        {
            var childID = GetChildTail(parent);
            var child = childID?.GetEntity(parent.Level);
            if (child == null)
                return parent;
            return FindTail(child);
        }
        public static void GetFullSnake(Entity segment, List<Entity> entities)
        {
            var tail = FindTail(segment);
            if (tail == null)
                return;
            entities.Add(tail);
            GetAllParents(tail, entities);
        }
        public static void GetAllParents(Entity segment, List<Entity> entities)
        {
            var parent = segment.Parent;
            if (parent == null || entities.Contains(parent))
                return;
            entities.Add(parent);
            GetAllParents(parent, entities);
        }
        #endregion 事件
        public static EntityID GetChildTail(Entity entity) => entity.GetBehaviourField<EntityID>(PROP_CHILD_TAIL);
        public static void SetChildTail(Entity entity, EntityID value) => entity.SetBehaviourField(PROP_CHILD_TAIL, value);
        public static int GetTargetGridIndex(Entity entity) => entity.GetBehaviourField<int>(PROP_TARGET_GRID_INDEX);
        public static void SetTargetGridIndex(Entity entity, int value) => entity.SetBehaviourField(PROP_TARGET_GRID_INDEX, value);

        #region 常量
        public const float KILL_SNAKE_DISTANCE = 32;
        public const float COLLIDE_SELF_DAMAGE = 600;
        private static readonly VanillaEntityPropertyMeta<EntityID> PROP_CHILD_TAIL = new VanillaEntityPropertyMeta<EntityID>("ChildTail");
        private static readonly VanillaEntityPropertyMeta<int> PROP_TARGET_GRID_INDEX = new VanillaEntityPropertyMeta<int>("TargetGridIndex");
        private static NamespaceID[] transferDamageExtraEffects = new NamespaceID[] { VanillaDamageEffects.TRANSFERRED, VanillaDamageEffects.NO_DAMAGE_BLINK };
        #endregion
    }
}
