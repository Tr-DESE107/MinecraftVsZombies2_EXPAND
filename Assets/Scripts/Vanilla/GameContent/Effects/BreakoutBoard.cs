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
using Tools.Mathematics;
using UnityEngine;

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
            var newVelocity = targetVelocity.normalized * velocity.magnitude;
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
            var screenPosition = level.GetPointerPosition();
            var size = entity.GetScaledSize();
            var position = level.ScreenToLawnPositionByY(screenPosition, 32);
            position.x = Mathf.Clamp(position.x, MIN_X + size.x * 0.5f, MAX_X - size.x * 0.5f);
            position.z = Mathf.Clamp(position.z, level.GetGridBottomZ() + size.z * 0.5f, level.GetGridTopZ() - size.z * 0.5f);
            entity.Position = position;

            var target = entity.Target;
            if (target != null && target.Exists())
            {
                target.Position = entity.Position + Vector3.right * 40;
                target.Velocity = Vector3.zero;
            }
            else
            {
                var pearls = level.FindEntities(VanillaProjectileID.breakoutPearl);
                if (pearls.Length <= 0)
                {
                    var pearl = level.Spawn(VanillaProjectileID.breakoutPearl, entity.Position + Vector3.right * 40, entity);
                    entity.Target = pearl;
                }
            }
        }
        #endregion
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
            var pearl = entity.Target;
            if (pearl != null && pearl.Exists())
            {
                entity.Target = null;
                pearl.Velocity = Vector3.right * 20;
            }
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
        public const float MAX_X = VanillaLevelExt.RIGHT_BORDER - 40;
        public const float MIN_X = VanillaLevelExt.LEFT_BORDER + 40;
    }
}