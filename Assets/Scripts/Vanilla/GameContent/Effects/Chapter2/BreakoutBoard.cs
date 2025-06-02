using System.Collections.Generic;
using MVZ2.GameContent.Buffs.Effects;
using MVZ2.GameContent.Projectiles;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Callbacks;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Level;
using MVZ2.Vanilla.Properties;
using MVZ2Logic;
using MVZ2Logic.Level;
using PVZEngine;
using PVZEngine.Callbacks;
using PVZEngine.Entities;
using PVZEngine.Level;
using UnityEngine;

namespace MVZ2.GameContent.Effects
{
    [EntityBehaviourDefinition(VanillaEffectNames.breakoutBoard)]
    public class BreakoutBoard : EffectBehaviour
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
        public override void Update(Entity entity)
        {
            base.Update(entity);
            var level = entity.Level;

            var nextDisplacement = GetBoardNextDisplacement(entity);
            if (nextDisplacement.sqrMagnitude > 0)
            {
                entity.Position += nextDisplacement;
                SetBoardNextDisplacement(entity, Vector3.zero);
            }

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

            bulletBuffer.Clear();
            level.FindEntitiesNonAlloc(e => e != entity.Target && entity.IsFriendly(e) && e.IsEntityOf(VanillaProjectileID.breakoutPearl), bulletBuffer);
            foreach (var pearl in bulletBuffer)
            {
                ResolveCollision(entity, pearl, entity.Position - entity.PreviousPosition);
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
        private void PostPointerActionCallback(VanillaCallbacks.PostPointerActionParams param, CallbackResult result)
        {
            var type = param.type;
            var phase = param.phase;
            var screenPosition = param.screenPos;
            var button = param.button;
            var delta = param.delta;
            if (!Global.Game.IsInLevel())
                return;
            var level = Global.Game.GetLevel();
            if (!level.IsGameRunning())
                return;
            boardsBuffer.Clear();
            level.FindEntitiesNonAlloc(e => e.IsEntityOf(VanillaEffectID.breakoutBoard), boardsBuffer);
            foreach (var board in boardsBuffer)
            {
                Vector3 position = board.Position + GetBoardNextDisplacement(board);
                if (type == PointerTypes.TOUCH)
                {
                    if (phase == PointerPhase.Press || phase == PointerPhase.Hold)
                    {
                        var lastScreenPosition = screenPosition - delta;
                        var pointerPosition = level.ScreenToLawnPositionByY(screenPosition, 32);
                        var lastPointerPosition = level.ScreenToLawnPositionByY(lastScreenPosition, 32);
                        position += pointerPosition - lastPointerPosition;
                    }
                }
                else if (type == PointerTypes.MOUSE)
                {
                    var pointerPosition = level.ScreenToLawnPositionByY(screenPosition, 32);
                    position = pointerPosition;
                }
                position.x = Mathf.Clamp(position.x, MIN_X, MAX_X);
                position.z = Mathf.Clamp(position.z, level.GetGridBottomZ(), level.GetGridTopZ());
                SetBoardNextDisplacement(board, position - board.Position);
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
        public static Vector3 GetBoardNextDisplacement(Entity board)
        {
            return board.GetBehaviourField<Vector3>(ID, PROP_NEXT_DISPLACEMENT);
        }
        public static void SetBoardNextDisplacement(Entity board, Vector3 value)
        {
            board.SetBehaviourField(ID, PROP_NEXT_DISPLACEMENT, value);
        }
        /// <summary>
        /// 计算碰撞后移动矩形B的位置，防止穿过静止矩形A
        /// </summary>
        public static void ResolveCollision(Entity board, Entity bullet, Vector3 boardDisplacement)
        {
            var currentA = board.GetBounds();
            var currentB = bullet.GetBounds();
            var prevA = currentA;
            prevA.center -= boardDisplacement;

            Vector3 velocity = bullet.Velocity - boardDisplacement;

            float collisionTime = SweptAABB(prevA, currentB, velocity, out var normal);

            if (collisionTime >= 1.0f)
            {
                return;
            }
            var finalPosition = currentB.center + velocity * collisionTime + boardDisplacement;
            bullet.SetCenter(finalPosition);

            // 设置子弹移速
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

            board.PlaySound(VanillaSoundID.reflection);
        }
        /// <summary>
        /// 执行扫掠式AABB碰撞检测
        /// 参数：
        ///   b：移动矩形
        ///   velocity：B在本帧的移动量（速度向量，假设时间步长为1）
        ///   a：静止矩形
        ///   normal：碰撞法向量（输出）
        /// 返回值：碰撞发生时的时间因子 t（0~1之间），若返回1表示本帧无碰撞
        /// </summary>
        public static float SweptAABB(Bounds a, Bounds b, Vector3 velocity, out Vector3 normal)
        {
            normal = Vector3.zero;

            Vector3 invEntry = Vector3.zero;
            Vector3 invExit = Vector3.zero;
            Vector3 entry = Vector3.zero;
            Vector3 exit = Vector3.zero;

            for (int i = 0; i < 3; i++)
            {
                // 计算沿X和Y方向的反向进入距离与离开距离
                if (velocity[i] > 0.0f)
                {
                    invEntry[i] = a.min[i] - b.max[i];
                    invExit[i] = a.max[i] - b.min[i];
                }
                else
                {
                    invEntry[i] = a.max[i] - b.min[i];
                    invExit[i] = a.min[i] - b.max[i];
                }
                // 计算进入时间与离开时间
                if (velocity[i] == 0.0f)
                {
                    entry[i] = float.NegativeInfinity;
                    exit[i] = float.PositiveInfinity;
                }
                else
                {
                    entry[i] = invEntry[i] / velocity[i];
                    exit[i] = invExit[i] / velocity[i];
                }
            }

            // 找到整体的进入时间与离开时间
            float entryTime = Mathf.Max(entry.x, entry.y, entry.z);
            float exitTime = Mathf.Min(exit.x, exit.y, exit.z);

            // 如果无碰撞，条件如下：
            // 1. 进入时间大于离开时间，说明两个矩形间存在分离
            // 2. 进入时间均为负，说明碰撞发生在上一帧
            // 3. 进入时间大于1，表示本帧内不会发生碰撞
            if (entryTime > exitTime || entryTime < 0 || entryTime > 1.0f)
            {
                return 1.0f;
            }

            // 根据哪个轴先碰撞确定碰撞法向量
            if (entryTime == entry.x)
            {
                normal = (invEntry.x < 0.0f) ? new Vector3(1, 0, 0) : new Vector3(-1, 0, 0);
            }
            else if (entryTime == entry.y)
            {
                normal = (invEntry.y < 0.0f) ? new Vector3(0, 1, 0) : new Vector3(0, -1, 0);
            }
            else
            {
                normal = (invEntry.z < 0.0f) ? new Vector3(0, 0, 1) : new Vector3(0, 0, -1);
            }

            return entryTime;
        }

        public static readonly NamespaceID ID = VanillaEffectID.breakoutBoard;
        public static readonly VanillaEntityPropertyMeta<int> PROP_RESPAWN_COUNTDOWN = new VanillaEntityPropertyMeta<int>("RespawnCountdown");
        public static readonly VanillaEntityPropertyMeta<Vector3> PROP_NEXT_DISPLACEMENT = new VanillaEntityPropertyMeta<Vector3>("NextDisplacement");
        public const int MAX_RESPAWN_COUNTDOWN = 90;
        public const float PEARL_SPEED = 15;
        public const float MAX_X = VanillaLevelExt.RIGHT_BORDER - 40;
        public const float MIN_X = VanillaLevelExt.LEFT_BORDER + 40;
        private List<Entity> boardsBuffer = new List<Entity>();
        private List<Entity> bulletBuffer = new List<Entity>();
    }
}