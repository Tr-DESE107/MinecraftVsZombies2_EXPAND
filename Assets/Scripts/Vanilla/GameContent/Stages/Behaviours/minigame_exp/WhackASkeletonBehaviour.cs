// 引用游戏逻辑和内容相关命名空间
using MVZ2.GameContent.Buffs.Enemies;
using MVZ2.GameContent.HeldItems;
using MVZ2.Vanilla.HeldItems;
using MVZ2Logic.Level;
using PVZEngine.Buffs;
using PVZEngine.Definitions;
using PVZEngine.Entities;
using PVZEngine.Level;
using UnityEngine;
using MVZ2Logic.HeldItems;

namespace MVZ2.GameContent.Stages
{
    /// <summary>
    /// “打幽灵”小游戏的关卡行为逻辑（Whack-A-Ghost）
    /// </summary>
    public class WhackASkeletonBehaviour : StageBehaviour
    {
        public WhackASkeletonBehaviour(StageDefinition stageDef) : base(stageDef) { }

        /// <summary>
        /// 关卡开始时触发：设置雷电定时器（每150帧一次）
        /// </summary>
        public override void Start(LevelEngine level)
        {
            //SetThunderTimer(level, new FrameTimer(150));
        }

        /// <summary>
        /// 每帧更新：自动给玩家发剑；定时触发雷电
        /// </summary>
        public override void Update(LevelEngine level)
        {
            // 如果当前未持有物品，则给玩家发一把剑
            if (level.GetHeldItemType() == BuiltinHeldTypes.none)
            {
                var builder = new HeldItemBuilder(VanillaHeldTypes.sword);
                builder.SetCannotCancel(true);
                level.SetHeldItem(builder);
            }

            //// 运行雷电计时器
            //var timer = GetThunderTimer(level);
            //timer.Run();

            //// 如果计时器到期，则触发一次雷电并重置计时器
            //if (timer.Expired)
            //{
            //    level.Thunder(); // 天空打雷
            //    timer.Reset();   // 重置计时器
            //}
        }

        /// <summary>
        /// 每波结束后触发
        /// - 缩短下一波雷电的冷却时间
        /// - 生成 Napstablook 幽灵敌人
        /// </summary>
        public override void PostWave(LevelEngine level, int wave)
        {
            base.PostWave(level, wave);

            // 缩短雷电冷却时间（最快30帧一次）
            //var timer = GetThunderTimer(level);
            //timer.Frame = Mathf.Min(timer.Frame, 30);

            // 根据当前波数计算生成 Napstablook 数量（第5波之后才开始）
            var napstablookPoints = (wave - 5) / 3f;

            // 如果是大型进攻波，数量翻倍
            if (level.IsHugeWave(wave))
            {
                napstablookPoints *= 2.5f;
            }

            // 向上取整为生成数量
            var napstablookCount = Mathf.CeilToInt(napstablookPoints);

            for (int i = 0; i < napstablookCount; i++)
            {
                // 随机选择一条敌人路径和一列
                var lane = level.GetRandomEnemySpawnLane();
                var column = level.GetSpawnRNG().Next(0, 5);

                // 获取对应位置坐标
                var x = level.GetColumnX(column);
                var z = level.GetEntityLaneZ(lane);
                var y = level.GetGroundY(x, z);

                // 设置生成参数：所属阵营为敌人阵营
                var spawnParam = new SpawnParams();
                spawnParam.SetProperty(EngineEntityProps.FACTION, level.Option.LeftFaction);

                // 生成 Napstablook 并给它加速 Buff
                //var napstablook = level.Spawn(VanillaEnemyID.napstablook, new Vector3(x, y, z), null, spawnParam);
                //AddSpeedBuff(napstablook);
            }
        }

        /// <summary>
        /// 敌人生成后触发：幽灵提前位移，加速所有敌人
        /// </summary>
        public override void PostEnemySpawned(Entity entity)
        {
            base.PostEnemySpawned(entity);

            // 如果是幽灵（ghost），随机向前推进一段距离
            //if (entity.IsEntityOf(VanillaEnemyID.ghost))
            //{
            //    var advanceDistance = entity.RNG.Next(0, entity.Level.GetGridWidth() * 3f);
            //    entity.Position += Vector3.left * advanceDistance;
            //}

            // 所有敌人添加速度 Buff（包括 Napstablook 和 ghost）
            AddSpeedBuff(entity);
        }

        /// <summary>
        /// 给敌人添加速度 Buff，速度倍率随波数增加
        /// </summary>
        private void AddSpeedBuff(Entity entity)
        {
            var buff = entity.AddBuff<MinigameEnemySpeedBuff>();
            float speedMultiplier = Mathf.Lerp(3, 5, entity.Level.CurrentWave / (float)entity.Level.GetTotalWaveCount());
            buff.SetProperty(MinigameEnemySpeedBuff.PROP_SPEED_MULTIPLIER, speedMultiplier);
        }

        #region 关卡属性

        //// 设置雷电计时器字段值
        //private void SetThunderTimer(LevelEngine level, FrameTimer timer)
        //    => level.SetBehaviourField(PROP_THUNDER_TIMER, timer);

        //// 获取雷电计时器字段值
        //private FrameTimer GetThunderTimer(LevelEngine level)
        //    => level.GetBehaviourField<FrameTimer>(PROP_THUNDER_TIMER);

        #endregion

        #region 属性字段定义

        // 属性注册区域名（用于关卡序列化）
        //private const string PROP_REGION = "whack_a_ghost";

        //// 注册一个关卡属性：雷电计时器
        //[LevelPropertyRegistry(PROP_REGION)]
        //public static readonly VanillaLevelPropertyMeta<FrameTimer> PROP_THUNDER_TIMER =
        //    new VanillaLevelPropertyMeta<FrameTimer>("ThunderTimer");

        #endregion
    }
}
