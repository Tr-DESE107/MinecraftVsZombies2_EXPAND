using System.Collections.Generic;
using System.Linq;
using MVZ2.GameContent.Buffs;
using MVZ2.GameContent.Buffs.Projectiles;
using MVZ2.GameContent.Detections;
using MVZ2.GameContent.Effects;
using MVZ2.GameContent.HeldItems;
using MVZ2.GameContent.Models;
using MVZ2.HeldItems;
using MVZ2.Vanilla;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Detections;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Level;
using MVZ2Logic;
using MVZ2Logic.HeldItems;
using MVZ2Logic.Level;
using MVZ2Logic.Models;
using PVZEngine;
using PVZEngine.Auras;
using PVZEngine.Buffs;
using PVZEngine.Entities;
using PVZEngine.Level;
using PVZEngine.SeedPacks;
using PVZEngine.Triggers;
using UnityEngine;

namespace MVZ2.GameContent.Contraptions
{
    [Definition(VanillaContraptionNames.forcePad)]
    public class ForcePad : ContraptionBehaviour, IStackEntity, IEntityHeldItemBehaviour
    {
        public ForcePad(string nsp, string name) : base(nsp, name)
        {
            AddAura(new DragAura());
            enemyDetector = new ForcePadDetector(EntityCollisionHelper.MASK_ENEMY, AFFECT_HEIGHT, 1);
            projectileDetector = new ForcePadDetector(EntityCollisionHelper.MASK_PROJECTILE, AFFECT_HEIGHT, 0.5f);
        }
        public override void Init(Entity entity)
        {
            base.Init(entity);
            entity.SetSortingLayer(SortingLayers.carriers);
        }
        protected override void UpdateAI(Entity entity)
        {
            base.UpdateAI(entity);
            UpdateGeneralAbility(entity);
        }
        protected override void UpdateLogic(Entity entity)
        {
            base.UpdateLogic(entity);
            if (entity.IsEvoked())
            {
                EvokedUpdate(entity);
            }
            entity.SetAnimationBool("IsOn", !entity.IsAIFrozen());
            entity.SetAnimationInt("State", GetAnimationState(entity));
        }
        protected override void OnEvoke(Entity entity)
        {
            base.OnEvoke(entity);
            // 开始拉取敌人。
            SetDragTarget(entity, Vector3.zero);
            SetDragTargetLocked(entity, false);
            var targets = entity.Level.FindEntities(e => CanDrag(entity, e));
            if (targets.Length > 0)
            {
                var level = entity.Level;
                var lines = new EntityID[targets.Length];
                for (int i = 0; i < targets.Length; i++)
                {
                    var target = targets[i];
                    var line = level.Spawn(VanillaEffectID.magneticLine, entity.Position, entity);
                    line.SetParent(entity);
                    line.Target = target;
                    lines[i] = new EntityID(line);
                }
                SetDraggingLines(entity, lines);
                SetDraggingEntities(entity, targets.Select(e => new EntityID(e)).ToArray());
                SetDragTimeout(entity, MAX_DRAG_TIMEOUT);
                entity.SetEvoked(true);
                entity.Level.SetHeldItem(VanillaHeldTypes.entity, entity.ID, 100, true);
            }
            else
            {
                SetDraggingLines(entity, null);
                SetDraggingEntities(entity, null);
                SetDragTimeout(entity, 0);
            }
            entity.PlaySound(VanillaSoundID.magnetic);
        }
        protected override void OnTrigger(Entity entity)
        {
            base.OnTrigger(entity);
            var padDirection = GetPadDirection(entity);
            padDirection = (padDirection + 1) % 4;
            SetPadDirection(entity, padDirection);
            var affectedEntities = GetAffectedEntities(entity);
            if (affectedEntities != null)
            {
                affectedEntities.Clear();
            }
            var buff = entity.AddBuff<WhiteFlashBuff>();
            buff.SetProperty(WhiteFlashBuff.PROP_TIMEOUT, 15);
            buff.SetProperty(WhiteFlashBuff.PROP_MAX_TIMEOUT, 15);
            entity.PlaySound(VanillaSoundID.wakeup);
        }
        private void UpdateGeneralAbility(Entity entity)
        {
            var level = entity.Level;
            var affectedEntities = GetAffectedEntities(entity);
            if (affectedEntities == null)
            {
                affectedEntities = new List<EntityID>();
                SetAffectedEntities(entity, affectedEntities);
            }
            detectBuffer.Clear();
            enemyDetector.DetectEntities(entity, detectBuffer);
            projectileDetector.DetectEntities(entity, detectBuffer);
            foreach (Entity target in detectBuffer)
            {
                bool start = false;
                if (!affectedEntities.Exists(e => e.ID == target.ID))
                {
                    start = true;
                    affectedEntities.Add(new EntityID(target));
                }
                AffectEntity(entity, target, start);
            }
            affectedEntities.RemoveAll(e => !detectBuffer.Contains(e.GetEntity(level)));
        }
        private void AffectEntity(Entity pad, Entity target, bool start)
        {
            if (target.Type == EntityTypes.ENEMY)
            {
                AffectEnemyWalkedOn(pad, target, start);
            }
            else if (target.Type == EntityTypes.PROJECTILE)
            {
                AffectProjectile(pad, target, start);
            }
        }
        private void AffectEnemyWalkedOn(Entity pad, Entity target, bool start)
        {
            var direction = GetPadDirection(pad);
            var up = direction == DIRECTION_UP;
            var down = direction == DIRECTION_DOWN;
            var right = direction == DIRECTION_RIGHT;
            var left = direction == DIRECTION_LEFT;
            if (left || right)
            {
                // Horizontal.
                float facingX = pad.GetFacingX();
                var pointX = (right ? 1 : -1) * facingX;
                target.Position += MOVE_ENEMY_SPEED * facingX * new Vector3(pointX, 0, 0);
            }
            else if (up || down)
            {
                // Vertical.
                var pointZ = (up ? 1 : -1);
                if (start)
                {
                    target.StartChangingLane(pad.GetLane() - pointZ);
                }
            }
        }
        private void AffectProjectile(Entity pad, Entity projectile, bool start)
        {
            if (!start)
                return;
            var direction = GetPadDirection(pad);
            Vector3 projVelocity = projectile.Velocity;

            var up = direction == DIRECTION_UP;
            var down = direction == DIRECTION_DOWN;
            var right = direction == DIRECTION_RIGHT;
            var left = direction == DIRECTION_LEFT;
            bool sameDirection = false;
            Vector3 velocityDir = Vector3.right;
            if (left || right)
            {
                // Horizontal.
                float facingX = pad.GetFacingX();
                var pointX = (right ? 1 : -1) * facingX;
                sameDirection = projVelocity.x * pointX > 0;
                velocityDir = new Vector3(pointX, 0, 0);
            }
            else if (up || down)
            {
                // Vertical.
                var pointZ = (up ? 1 : -1);
                sameDirection = projVelocity.z * pointZ > 0;
                velocityDir = new Vector3(0, 0, pointZ);
            }

            // 相同方向，增加击退效果并提升弹速
            if (sameDirection)
            {
                if (!projectile.HasBuff<ProjectileKnockbackBuff>())
                    projectile.AddBuff<ProjectileKnockbackBuff>();
                projectile.Velocity += PROJECTILE_SPEED_BOOST * velocityDir;
            }
            else
            {
                // 不同方向，转移方向。
                projectile.Velocity = velocityDir * projectile.Velocity.magnitude;
            }

        }
        private void EvokedUpdate(Entity pad)
        {
            // 正在拖动目标。
            bool locked = IsDragTargetLocked(pad);
            Vector3 targetPosition = GetDragTarget(pad);
            var draggingEntities = GetDraggingEntities(pad);
            bool hasValidEntity = false;
            if (draggingEntities != null)
            {
                foreach (var targetID in draggingEntities)
                {
                    var target = targetID.GetEntity(pad.Level);
                    // 不存在或者死亡的目标被排除在外。
                    if (target == null || !target.Exists() || target.IsDead)
                        continue;
                    hasValidEntity = true;

                    // 拖动目标到指定位置。
                    var pos = target.Position;
                    if (locked)
                    {
                        pos = targetPosition;
                    }
                    pos.y = pad.Level.GetGroundY(pos.x, pos.z) + 10;
                    target.Position = target.Position * 0.5f + pos * 0.5f;
                }
            }
            // 拖动倒计时。
            var dragTimeout = GetDragTimeout(pad);
            dragTimeout--;
            SetDragTimeout(pad, dragTimeout);

            // 倒计时结束，或者没有在手持该器械，或者已经没有有效的实体了
            // 结束大招。
            bool holdingThis = pad.Level.IsHoldingEntity(pad);
            if (dragTimeout <= 0 || (!holdingThis && !locked) || !hasValidEntity)
            {
                if (holdingThis)
                {
                    pad.Level.ResetHeldItem();
                }
                pad.SetEvoked(false);

                // 删除所有连接线。
                var draggingLines = GetDraggingLines(pad);
                if (draggingLines != null)
                {
                    foreach (var lineID in draggingLines)
                    {
                        var line = lineID.GetEntity(pad.Level);
                        if (line == null)
                            continue;
                        line.Remove();
                    }
                }
                SetDraggingLines(pad, null);
                SetDraggingEntities(pad, null);
                SetDragTimeout(pad, 0);
                SetDragTarget(pad, Vector3.zero);
                SetDragTargetLocked(pad, false);
            }
        }
        private bool CanDrag(Entity self, Entity target)
        {
            return !target.IsDead && self.IsHostile(target) && Vector3.Distance(self.Position, target.Position) < DRAG_RADIUS;
        }

        #region 属性
        public static int GetPadDirection(Entity pad) => pad.GetBehaviourField<int>(ID, PROP_PAD_DIRECTION);
        public static void SetPadDirection(Entity pad, int direction) => pad.SetBehaviourField(ID, PROP_PAD_DIRECTION, direction);
        public static List<EntityID> GetAffectedEntities(Entity pad) => pad.GetBehaviourField<List<EntityID>>(ID, PROP_AFFECTED_ENTITIES);
        public static void SetAffectedEntities(Entity pad, List<EntityID> value) => pad.SetBehaviourField(ID, PROP_AFFECTED_ENTITIES, value);

        #region 拖拽
        public static EntityID[] GetDraggingEntities(Entity pad) => pad.GetBehaviourField<EntityID[]>(ID, PROP_DRAGGING_ENTITIES);
        public static void SetDraggingEntities(Entity pad, EntityID[] value) => pad.SetBehaviourField(ID, PROP_DRAGGING_ENTITIES, value);
        public static EntityID[] GetDraggingLines(Entity pad) => pad.GetBehaviourField<EntityID[]>(ID, PROP_DRAGGING_LINES);
        public static void SetDraggingLines(Entity pad, EntityID[] value) => pad.SetBehaviourField(ID, PROP_DRAGGING_LINES, value);
        public static bool IsDragTargetLocked(Entity pad) => pad.GetBehaviourField<bool>(ID, PROP_DRAG_TARGET_LOCKED);
        public static void SetDragTargetLocked(Entity pad, bool value) => pad.SetBehaviourField(ID, PROP_DRAG_TARGET_LOCKED, value);
        public static Vector3 GetDragTarget(Entity pad) => pad.GetBehaviourField<Vector3>(ID, PROP_DRAG_TARGET);
        public static void SetDragTarget(Entity pad, Vector3 position) => pad.SetBehaviourField(ID, PROP_DRAG_TARGET, position);
        public static int GetDragTimeout(Entity pad) => pad.GetBehaviourField<int>(ID, PROP_DRAG_TIMEOUT);
        public static void SetDragTimeout(Entity pad, int value) => pad.SetBehaviourField(ID, PROP_DRAG_TIMEOUT, value);
        #endregion

        #endregion
        private int GetAnimationState(Entity pad)
        {
            if (pad.IsEvoked())
                return 4;
            return GetPadDirection(pad);
        }

        void IStackEntity.CanStackOnEntity(Entity target, TriggerResultBoolean result)
        {
            if (!target.IsEntityOf(VanillaContraptionID.gravityPad))
                return;
            result.Result = true;
        }

        void IStackEntity.StackOnEntity(Entity target)
        {
            if (!target.IsEntityOf(VanillaContraptionID.gravityPad))
                return;
            target.UpgradeToContraption(VanillaContraptionID.forcePad);
        }
        public const string PROP_PAD_DIRECTION = "PadDirection";
        public const string PROP_AFFECTED_ENTITIES = "AffectedEntities";
        public const string PROP_DRAG_TARGET = "DragTarget";
        public const string PROP_DRAG_TARGET_LOCKED = "DragTargetLocked";
        public const string PROP_DRAGGING_LINES = "DraggingLines";
        public const string PROP_DRAGGING_ENTITIES = "DraggingEntities";
        public const string PROP_DRAG_TIMEOUT = "DragTimeout";
        public const float DRAG_RADIUS = 150;
        public const int MAX_DRAG_TIMEOUT = 150;

        public const float AFFECT_HEIGHT = 64;
        public const float MIN_HEIGHT = 5;
        public const float MOVE_ENEMY_SPEED = 0.233333f;
        public const int DIRECTION_RIGHT = 0;
        public const int DIRECTION_DOWN = 1;
        public const int DIRECTION_LEFT = 2;
        public const int DIRECTION_UP = 3;
        public const float PROJECTILE_SPEED_BOOST = 1;
        public static readonly NamespaceID ID = VanillaContraptionID.forcePad;
        private Detector enemyDetector;
        private Detector projectileDetector;
        private List<Entity> detectBuffer = new List<Entity>();

        bool IEntityHeldItemBehaviour.CheckRaycast(Entity entity, HeldItemTarget target, IHeldItemData data)
        {
            return target is HeldItemTargetGrid targetGrid;
        }

        HeldHighlight IEntityHeldItemBehaviour.GetHighlight(Entity entity, HeldItemTarget target, IHeldItemData data)
        {
            return HeldHighlight.Green;
        }

        void IEntityHeldItemBehaviour.Use(Entity entity, HeldItemTarget target, IHeldItemData data, PointerInteraction interaction)
        {
            var targetPhase = Global.IsMobile() ? PointerInteraction.Release : PointerInteraction.Press;
            if (interaction != targetPhase)
                return;
            if (target is not HeldItemTargetGrid targetGrid)
                return;
            if (targetGrid.Target == null)
                return;
            var level = target.GetLevel();
            SetDragTargetLocked(entity, true);
            SetDragTarget(entity, targetGrid.Target.GetEntityPosition());
            SetDragTimeout(entity, 30);
            level.ResetHeldItem();
            entity.PlaySound(VanillaSoundID.magnetic);
        }

        SeedPack IEntityHeldItemBehaviour.GetSeedPack(Entity entity, LevelEngine level, IHeldItemData data)
        {
            return null;
        }

        NamespaceID IEntityHeldItemBehaviour.GetModelID(Entity entity, LevelEngine level, IHeldItemData data)
        {
            return VanillaModelID.targetHeldItem;
        }

        float IEntityHeldItemBehaviour.GetRadius(Entity entity, LevelEngine level, IHeldItemData data)
        {
            return 0;
        }

        void IEntityHeldItemBehaviour.Update(Entity entity, LevelEngine level, IHeldItemData data)
        {
            if (entity == null || !entity.Exists() || entity.IsAIFrozen())
            {
                level.ResetHeldItem();
                return;
            }
        }

        public class DragAura : AuraEffectDefinition
        {
            public DragAura()
            {
                BuffID = VanillaBuffID.forcePadDrag;
            }

            public override void GetAuraTargets(AuraEffect auraEffect, List<IBuffTarget> results)
            {
                var sourceEnt = auraEffect.Source?.GetEntity();
                if (sourceEnt == null)
                    return;
                if (!sourceEnt.IsEvoked())
                    return;
                var draggingEntities = GetDraggingEntities(sourceEnt);
                if (draggingEntities == null)
                    return;
                results.AddRange(draggingEntities.Select(e => e.GetEntity(sourceEnt.Level)).Where(e => e != null && e.Exists() && !e.IsDead));
            }
        }
    }
}
