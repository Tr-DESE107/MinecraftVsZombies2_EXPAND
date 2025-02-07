using System.Collections.Generic;
using MVZ2.GameContent.Buffs.Effects;
using MVZ2.GameContent.HeldItems;
using MVZ2.GameContent.Projectiles;
using MVZ2.HeldItems;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Callbacks;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Level;
using MVZ2.Vanilla.Properties;
using MVZ2Logic;
using MVZ2Logic.HeldItems;
using MVZ2Logic.Level;
using PVZEngine;
using PVZEngine.Entities;
using PVZEngine.Level;
using PVZEngine.SeedPacks;
using Tools.Mathematics;
using UnityEngine;

namespace MVZ2.GameContent.Effects
{
    [EntityBehaviourDefinition(VanillaEffectNames.breakoutBoard)]
    public class BreakoutBoard : EffectBehaviour, IEntityHeldItemBehaviour
    {

        #region 公有方法
        public BreakoutBoard(string nsp, string name) : base(nsp, name)
        {
            AddTrigger(VanillaCallbacks.POST_POINTER_ACTION, PostPointerActionCallback);
        }
        public override void Init(Entity entity)
        {
            base.Init(entity);
            entity.CollisionMaskFriendly = EntityCollisionHelper.MASK_PROJECTILE;
        }
        public override void PostCollision(EntityCollision collision, int state)
        {
            base.PostCollision(collision, state);
            if (!collision.Collider.IsMain() || !collision.OtherCollider.IsMain())
                return;
            if (state == EntityCollisionHelper.STATE_EXIT)
                return;
            var bullet = collision.Other;
            if (!bullet.IsEntityOf(VanillaProjectileID.breakoutPearl))
                return;
            var board = collision.Entity;
            if (bullet == board.Target)
                return;
            if (!board.IsFriendly(bullet))
                return;
            PushBullet(board, bullet);

            board.PlaySound(VanillaSoundID.reflection);

        }
        public override void Update(Entity entity)
        {
            base.Update(entity);
            var level = entity.Level;

            bool pearlExists = true;
            var target = entity.Target;
            if (target != null && target.Exists())
            {
                var targetPosition = entity.Position + Vector3.right * 40;
                if (target.State == VanillaEntityStates.BREAKOUT_PEARL_RETURN)
                {
                    target.Velocity = (targetPosition - target.Position) * 0.5f;
                }
                else if (target.State == VanillaEntityStates.BREAKOUT_PEARL_IDLE)
                {
                    target.Position = targetPosition;
                    target.Velocity = Vector3.zero;
                }
            }
            else
            {
                if (!level.EntityExists(VanillaProjectileID.breakoutPearl))
                {
                    pearlExists = false;
                }
            }
            if (!pearlExists)
            {
                var countdown = GetRespawnCountdown(entity);
                countdown--;
                if (countdown <= 0)
                {
                    countdown = MAX_RESPAWN_COUNTDOWN;
                    SpawnPearl(entity);
                    entity.SetModelProperty("Countdown", 0);
                }
                else
                {
                    entity.SetModelProperty("Countdown", countdown);
                }
                SetRespawnCountdown(entity, countdown);
            }
            else
            {
                SetRespawnCountdown(entity, MAX_RESPAWN_COUNTDOWN);
                entity.SetModelProperty("Countdown", 0);
            }
            entity.SetAnimationBool("Upgraded", IsUpgraded(entity));
        }
        private void PostPointerActionCallback(int type, int index, Vector2 screenPosition, PointerPhase phase)
        {
            if (!Global.Game.IsInLevel())
                return;
            var level = Global.Game.GetLevel();
            if (!level.IsGameRunning())
                return;
            boardsBuffer.Clear();
            level.FindEntitiesNonAlloc(e => e.IsEntityOf(VanillaEffectID.breakoutBoard), boardsBuffer);
            foreach (var board in boardsBuffer)
            {
                Vector3 position = board.Position;
                if (type == PointerTypes.TOUCH)
                {
                    if (phase == PointerPhase.Press || phase == PointerPhase.Hold)
                    {
                        var touchDelta = Global.GetTouchDelta(index);
                        var lastScreenPosition = screenPosition - touchDelta;
                        var pointerPosition = level.ScreenToLawnPositionByY(screenPosition, 32);
                        var lastPointerPosition = level.ScreenToLawnPositionByY(lastScreenPosition, 32);
                        position = board.Position + pointerPosition - lastPointerPosition;
                    }
                }
                else if (type == PointerTypes.MOUSE)
                {
                    var pointerPosition = level.ScreenToLawnPositionByY(screenPosition, 32);
                    position = pointerPosition;
                }
                position.x = Mathf.Clamp(position.x, MIN_X, MAX_X);
                position.z = Mathf.Clamp(position.z, level.GetGridBottomZ(), level.GetGridTopZ());
                board.Position = position;
            }
        }
        #endregion
        public static Entity SpawnPearl(Entity board)
        {
            var level = board.Level;
            var pearl = level.Spawn(VanillaProjectileID.breakoutPearl, board.Position + Vector3.right * 40, board);
            board.Target = pearl;
            pearl.SetParent(board);
            board.State = VanillaEntityStates.BREAKOUT_PEARL_IDLE;
            return pearl;
        }
        public static void ReturnPearl(Entity board, Entity pearl)
        {
            var level = board.Level;
            board.Target = pearl;
            pearl.SetParent(board);
            board.State = VanillaEntityStates.BREAKOUT_PEARL_RETURN;
        }
        public static void FirePearl(Entity board)
        {
            var pearl = board.Target;
            if (pearl != null && pearl.Exists())
            {
                board.Target = null;
                pearl.SetParent(null);
                pearl.Velocity = Vector3.right * PEARL_SPEED;
                board.State = VanillaEntityStates.BREAKOUT_PEARL_FIRED;
            }
        }
        public static bool IsUpgraded(Entity board)
        {
            return board.HasBuff<BreakoutBoardUpgradeBuff>();
        }
        public static void Upgrade(Entity board)
        {
            if (!IsUpgraded(board))
                board.AddBuff<BreakoutBoardUpgradeBuff>();
        }
        public static int GetRespawnCountdown(Entity board)
        {
            return board.GetBehaviourField<int>(ID, PROP_RESPAWN_COUNTDOWN);
        }
        public static void SetRespawnCountdown(Entity board, int value)
        {
            board.SetBehaviourField(ID, PROP_RESPAWN_COUNTDOWN, value);
        }
        private void PushBullet(Entity board, Entity bullet)
        {
            // 挤开子弹。
            // 获取板子和子弹的碰撞箱。
            var boardBounds = board.GetBounds();
            var bulletBounds = bullet.GetBounds();

            var bulletPos = bulletBounds.center;
            var boardPos = boardBounds.center;
            // 获取子弹和板子体积相加的值。
            var extentsX = boardBounds.extents.x + bulletBounds.extents.x;
            var extentsZ = boardBounds.extents.z + bulletBounds.extents.z;

            // 从子弹的上一处位置相对于板子上一处的位置，向当前位置相对于板子的位置画一条线段。
            var bulletLineStart = bullet.PreviousPosition - board.PreviousPosition;
            var bulletLineEnd = bullet.Position - board.Position;

            // X方向不相交。
            if (!MathTool.DoRangesIntersect(bulletLineStart.x, bulletLineEnd.x, -extentsX, extentsX))
                return;
            // Z方向不相交。
            if (!MathTool.DoRangesIntersect(bulletLineStart.z, bulletLineEnd.z, -extentsZ, extentsZ))
                return;

            var bulletLine = bulletLineEnd - bulletLineStart;
            // 获取相交前，X轴和Z轴的有效移动距离。
            float hitLineLengthX;
            if (bulletLineStart.x < 0)
            {
                hitLineLengthX = Mathf.Min(bulletLineEnd.x, -extentsX) - bulletLineStart.x;
            }
            else
            {
                hitLineLengthX = bulletLineStart.x - Mathf.Max(bulletLineEnd.x, extentsX);
            }
            float hitLineLengthZ;
            if (bulletLineStart.z < 0)
            {
                hitLineLengthZ = Mathf.Min(bulletLineEnd.z, -extentsZ) - bulletLineStart.z;
            }
            else
            {
                hitLineLengthZ = bulletLineStart.z - Mathf.Max(bulletLineEnd.z, extentsZ);
            }

            // 获取在X轴和Z轴上的有效移动距离的有效百分比。
            float bulletHitPercent = 0;
            if (bulletLine.x != 0 && bulletLine.z != 0)
            {
                if (hitLineLengthX >= 0 && hitLineLengthZ >= 0)
                {
                    // 两个轴全部接触，取距离最近的
                    float linePercentX = hitLineLengthX / Mathf.Abs(bulletLine.x);
                    float linePercentZ = hitLineLengthZ / Mathf.Abs(bulletLine.z);
                    bulletHitPercent = Mathf.Min(linePercentX, linePercentZ);
                }
                else if (hitLineLengthZ >= 0)
                {
                    // Z轴接触
                    bulletHitPercent = hitLineLengthZ / Mathf.Abs(bulletLine.z);
                }
                else if (hitLineLengthX >= 0)
                {
                    // X轴接触
                    bulletHitPercent = hitLineLengthX / Mathf.Abs(bulletLine.x);
                }
                else
                {
                    // 两个轴全在内部，穿模了
                    return;
                }
            }
            else if (bulletLine.x != 0)
            {
                bulletHitPercent = hitLineLengthX / Mathf.Abs(bulletLine.x);
            }
            else if (bulletLine.z != 0)
            {
                bulletHitPercent = hitLineLengthZ / Mathf.Abs(bulletLine.z);
            }
            else
            {
                // 根本没有在移动
                return;
            }

            var bulletHitOffset = bulletLine * bulletHitPercent + bulletLineStart;
            var finalPosition = boardPos + bulletHitOffset;
            finalPosition.y = bulletPos.y;
            bullet.SetCenter(finalPosition);



            var boardDisplacement = board.Position - board.PreviousPosition;
            var targetVelocity = bullet.Position - board.Position;
            if (targetVelocity.x * boardDisplacement.x > 0)
            {
                targetVelocity.x += boardDisplacement.x;
            }
            if (targetVelocity.z * boardDisplacement.z > 0)
            {
                targetVelocity.z += boardDisplacement.z;
            }
            targetVelocity.y = 0;
            bullet.Velocity = targetVelocity.normalized * PEARL_SPEED;

        }

        bool IEntityHeldItemBehaviour.CheckRaycast(Entity entity, HeldItemTarget target, IHeldItemData data)
        {
            return target is HeldItemTargetLawn targetLawn && targetLawn.Area == LawnArea.Main;
        }

        HeldHighlight IEntityHeldItemBehaviour.GetHighlight(Entity entity, HeldItemTarget target, IHeldItemData data)
        {
            return HeldHighlight.None;
        }

        void IEntityHeldItemBehaviour.Use(Entity entity, HeldItemTarget target, IHeldItemData data, PointerInteraction interaction)
        {
            var targetPhase = Global.IsMobile() ? PointerInteraction.Release : PointerInteraction.Press;
            if (interaction != targetPhase)
                return;
            FirePearl(entity);
        }

        SeedPack IEntityHeldItemBehaviour.GetSeedPack(Entity entity, LevelEngine level, IHeldItemData data)
        {
            return null;
        }

        NamespaceID IEntityHeldItemBehaviour.GetModelID(Entity entity, LevelEngine level, IHeldItemData data)
        {
            return null;
        }

        float IEntityHeldItemBehaviour.GetRadius(Entity entity, LevelEngine level, IHeldItemData data)
        {
            return 0;
        }

        void IEntityHeldItemBehaviour.Update(Entity entity, LevelEngine level, IHeldItemData data)
        {
        }
        public static readonly NamespaceID ID = VanillaEffectID.breakoutBoard;
        public static readonly VanillaEntityPropertyMeta PROP_RESPAWN_COUNTDOWN = new VanillaEntityPropertyMeta("RespawnCountdown");
        public const int MAX_RESPAWN_COUNTDOWN = 90;
        public const float PEARL_SPEED = 15;
        public const float MAX_X = VanillaLevelExt.RIGHT_BORDER - 40;
        public const float MIN_X = VanillaLevelExt.LEFT_BORDER + 40;
        private List<Entity> boardsBuffer = new List<Entity>();
    }
}