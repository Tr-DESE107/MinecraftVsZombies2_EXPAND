using MVZ2.GameContent.Buffs.Effects;
using MVZ2.GameContent.HeldItems;
using MVZ2.GameContent.Projectiles;
using MVZ2.HeldItems;
using MVZ2.Vanilla;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Level;
using MVZ2Logic;
using MVZ2Logic.HeldItems;
using MVZ2Logic.Level;
using PVZEngine;
using PVZEngine.Entities;
using PVZEngine.Level;
using PVZEngine.SeedPacks;
using UnityEngine;
using UnityEngine.UIElements;

namespace MVZ2.GameContent.Effects
{
    [Definition(VanillaEffectNames.breakoutBoard)]
    public class BreakoutBoard : EffectBehaviour, IEntityHeldItemBehaviour
    {

        #region 公有方法
        public BreakoutBoard(string nsp, string name) : base(nsp, name)
        {
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
            PushBullet(board, bullet, collision.Seperation);

            var boardVelocity = board.Position - board.PreviousPosition;
            var targetVelocity = bullet.Position - board.Position;
            if (targetVelocity.x * boardVelocity.x > 0)
            {
                targetVelocity.x += boardVelocity.x;
            }
            if (targetVelocity.z * boardVelocity.z > 0)
            {
                targetVelocity.z += boardVelocity.z;
            }
            targetVelocity.y = 0;

            var velocity = bullet.Velocity;
            var newVelocity = targetVelocity.normalized * PEARL_SPEED;
            velocity.x = newVelocity.x;
            velocity.y = 0;
            velocity.z = newVelocity.z;
            bullet.Velocity = velocity;

            board.PlaySound(VanillaSoundID.reflection);

        }
        public override void Update(Entity entity)
        {
            base.Update(entity);
            var level = entity.Level;
            Vector3 position;
            if (Global.IsMobile())
            {
                if (Global.GetTouchCount() > 0)
                {
                    var touchDelta = Global.GetTouchDelta(0);
                    var touchPosition = Global.GetTouchPosition(0);
                    var lastScreenPosition = touchPosition - touchDelta;
                    var pointerPosition = level.ScreenToLawnPositionByY(touchPosition, 32);
                    var lastPointerPosition = level.ScreenToLawnPositionByY(lastScreenPosition, 32);
                    position = entity.Position + pointerPosition - lastPointerPosition;
                }
                else
                {
                    position = entity.Position;
                }
            }
            else
            {
                var screenPosition = Global.GetPointerScreenPosition();
                var pointerPosition = level.ScreenToLawnPositionByY(screenPosition, 32);
                position = pointerPosition;
            }
            position.x = Mathf.Clamp(position.x, MIN_X, MAX_X);
            position.z = Mathf.Clamp(position.z, level.GetGridBottomZ(), level.GetGridTopZ());
            entity.Position = position;

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
                var pearls = level.FindEntities(VanillaProjectileID.breakoutPearl);
                if (pearls.Length <= 0)
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
            return board.GetBehaviourProperty<int>(ID, PROP_RESPAWN_COUNTDOWN);
        }
        public static void SetRespawnCountdown(Entity board, int value)
        {
            board.SetBehaviourProperty(ID, PROP_RESPAWN_COUNTDOWN, value);
        }
        private void PushBullet(Entity board, Entity bullet, Vector3 bullet2Board)
        {
            // 挤开子弹。
            // 获取板子和子弹的碰撞箱。
            var boardBounds = board.GetBounds();
            var bulletBounds = bullet.GetBounds();
            var bulletDisplacement = bullet.Position - bullet.PreviousPosition;
            var boardDisplacement = board.Position - board.PreviousPosition;
            var relativeDisplacement = bulletDisplacement - boardDisplacement;
            // 获取子弹和板子的数值最低和最高的两个角落。
            var boardMin = boardBounds.min;
            var boardMax = boardBounds.max;
            var bulletMin = bulletBounds.min;
            var bulletMax = bulletBounds.max;
            var bulletPos = bulletBounds.center;
            var boardPos = boardBounds.center;
            // 获取挤开所需要的最小数值。
            Vector3 impluse = Vector2.zero;
            if (bulletMin.x <= boardMax.x && bulletMin.x - relativeDisplacement.x > boardMax.x)
            {
                // 触碰到右边缘。
                impluse.x = boardMax.x - bulletMin.x;
            }
            else if (bulletMax.x >= boardMin.x && bulletMax.x - relativeDisplacement.x < boardMin.x)
            {
                // 触碰到左边缘。
                impluse.x = boardMin.x - bulletMax.x;
            }
            if (bulletMin.z <= boardMax.z && bulletMin.z - relativeDisplacement.z > boardMax.z)
            {
                impluse.z = boardMax.z - bulletMin.z;
            }
            else if (bulletMax.z >= boardMin.z && bulletMax.z - relativeDisplacement.z < boardMin.z)
            {
                impluse.z = boardMin.z - bulletMax.z;
            }
            // 计算挤开子弹所需要的额外距离。
            Vector3 finalImpluse;
            finalImpluse = new Vector3(impluse.x, 0, impluse.z);

            var finalDistance = bullet2Board + finalImpluse;

            bullet.SetCenter(board.GetCenter() + finalDistance);
        }

        bool IEntityHeldItemBehaviour.CheckRaycast(Entity entity, HeldItemTarget target, IHeldItemData data)
        {
            return target is HeldItemTargetLawn targetLawn && targetLawn.Area == LawnArea.Main;
        }

        HeldHighlight IEntityHeldItemBehaviour.GetHighlight(Entity entity, HeldItemTarget target, IHeldItemData data)
        {
            return HeldHighlight.None;
        }

        void IEntityHeldItemBehaviour.Use(Entity entity, HeldItemTarget target, IHeldItemData data, PointerPhase phase)
        {
            var targetPhase = Global.IsMobile() ? PointerPhase.Release : PointerPhase.Press;
            if (phase != targetPhase)
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
        public const string PROP_RESPAWN_COUNTDOWN = "RespawnCountdown";
        public const int MAX_RESPAWN_COUNTDOWN = 90;
        public const float PEARL_SPEED = 15;
        public const float MAX_X = VanillaLevelExt.RIGHT_BORDER - 40;
        public const float MIN_X = VanillaLevelExt.LEFT_BORDER + 40;
    }
}