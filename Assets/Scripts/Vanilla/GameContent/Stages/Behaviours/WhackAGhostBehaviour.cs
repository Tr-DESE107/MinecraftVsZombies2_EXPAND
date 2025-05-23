using MVZ2.GameContent.Buffs.Enemies;
using MVZ2.GameContent.Contraptions;
using MVZ2.GameContent.Enemies;
using MVZ2.GameContent.HeldItems;
using MVZ2.GameContent.Seeds;
using MVZ2.Vanilla.HeldItems;
using MVZ2.Vanilla.Level;
using MVZ2.Vanilla.Properties;
using MVZ2Logic.Level;
using PVZEngine;
using PVZEngine.Definitions;
using PVZEngine.Entities;
using PVZEngine.Level;
using Tools;
using UnityEngine;

namespace MVZ2.GameContent.Stages
{
    public class WhackAGhostBehaviour : StageBehaviour
    {
        public WhackAGhostBehaviour(StageDefinition stageDef) : base(stageDef)
        {
            stageDef.SetProperty(VanillaLevelProps.CONVEY_SPEED, 1);
        }
        public override void Start(LevelEngine level)
        {
            level.SetStarshardActive(false);

            level.SetNapstablookParalysisTime(45);
            SetThunderTimer(level, new FrameTimer(150));
        }
        public override void Update(LevelEngine level)
        {
            if (level.GetHeldItemType() == BuiltinHeldTypes.none)
            {
                level.SetHeldItem(VanillaHeldTypes.sword, 0, 0, true);
            }
            var timer = GetThunderTimer(level);
            timer.Run();
            if (timer.Expired)
            {
                level.Thunder();
                timer.Reset();
            }
        }
        public override void PostWave(LevelEngine level, int wave)
        {
            base.PostWave(level, wave);
            var timer = GetThunderTimer(level);
            timer.Frame = Mathf.Min(timer.Frame, 30);

            var napstablookPoints = (wave - 5) / 3f;
            if (level.IsHugeWave(wave))
            {
                napstablookPoints *= 2.5f;
            }
            var napstablookCount = Mathf.CeilToInt(napstablookPoints);
            for (int i = 0; i < napstablookCount; i++)
            {
                var lane = level.GetRandomEnemySpawnLane();
                var column = level.GetSpawnRNG().Next(0, 5);
                var x = level.GetColumnX(column);
                var z = level.GetEntityLaneZ(lane);
                var y = level.GetGroundY(x, z);
                var spawnParam = new SpawnParams();
                spawnParam.SetProperty(EngineEntityProps.FACTION, level.Option.LeftFaction);
                var napstablook = level.Spawn(VanillaEnemyID.napstablook, new Vector3(x, y, z), null, spawnParam);
                AddSpeedBuff(napstablook);
            }
        }
        public override void PostEnemySpawned(PVZEngine.Entities.Entity entity)
        {
            base.PostEnemySpawned(entity);
            if (entity.IsEntityOf(VanillaEnemyID.ghost))
            {
                var advanceDistance = entity.RNG.Next(0, entity.Level.GetGridWidth() * 3f);
                entity.Position += Vector3.left * advanceDistance;
            }
            AddSpeedBuff(entity);
        }

        private void AddSpeedBuff(Entity entity)
        {
            var buff = entity.AddBuff<MinigameEnemySpeedBuff>();
            buff.SetProperty(MinigameEnemySpeedBuff.PROP_SPEED_MULTIPLIER, Mathf.Lerp(3, 5, entity.Level.CurrentWave / (float)entity.Level.GetTotalWaveCount()));
        }
        #region 关卡属性
        private void SetThunderTimer(LevelEngine level, FrameTimer timer) => level.SetBehaviourField(PROP_THUNDER_TIMER, timer);
        private FrameTimer GetThunderTimer(LevelEngine level) => level.GetBehaviourField<FrameTimer>(PROP_THUNDER_TIMER);
        #endregion

        #region 属性字段
        private const string PROP_REGION = "whack_a_ghost";
        [LevelPropertyRegistry(PROP_REGION)]
        public static readonly VanillaLevelPropertyMeta PROP_THUNDER_TIMER = new VanillaLevelPropertyMeta("ThunderTimer");
        #endregion
    }
}
