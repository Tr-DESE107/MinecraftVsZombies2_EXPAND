using System.Linq;
using MVZ2.Extensions;
using MVZ2.GameContent;
using PVZEngine;
using PVZEngine.Definitions;
using PVZEngine.Level;
using Tools;
using UnityEngine;

namespace MVZ2.Vanilla
{
    public class WaveStageBehaviour : StageBehaviour
    {
        public WaveStageBehaviour(StageDefinition stageDef) : base(stageDef)
        {
            stageDef.SetProperty(BuiltinStageProps.WAVE_MAX_TIME, 900);
            stageDef.SetProperty(BuiltinStageProps.WAVE_ADVANCE_TIME, 300);
            stageDef.SetProperty(BuiltinStageProps.WAVE_ADVANCE_HEALTH_PERCENT, 0.6f);
        }
        public override void PrepareForBattle(LevelEngine level)
        {
            level.AreaDefinition.PrepareForBattle(level);
            if (level.Difficulty != LevelDifficulty.hard)
            {
                var cartRef = level.GetProperty<NamespaceID>(AreaProperties.CART_REFERENCE);
                level.SpawnCarts(cartRef, BuiltinLevel.CART_START_X, 20);
            }
        }
        public override void Start(LevelEngine level)
        {
            var time = level.CurrentFlag > 0 ? level.GetContinutedFirstWaveTime() : level.GetFirstWaveTime();
            var waveTimer = new FrameTimer(time);
            SetWaveTimer(level, waveTimer);
        }
        public override void Update(LevelEngine level)
        {
            switch (level.WaveState)
            {
                case STATE_NOT_STARTED:
                    NotStartedUpdate(level);
                    break;
                case STATE_STARTED:
                    StartedUpdate(level);
                    break;
                case STATE_HUGE_WAVE_APPROACHING:
                    HugeWaveApproachingUpdate(level);
                    break;
                case STATE_FINAL_WAVE:
                    FinalWaveUpdate(level);
                    break;
            }
            level.CheckGameOver();
        }
        public override void PostEnemySpawned(Entity entity)
        {
            AddWaveMaxHealth(entity.Level, entity.GetMaxHealth() + (entity.EquipedArmor?.GetMaxHealth() ?? 0));
        }

        #region 波次
        private void CheckWaveAdvancement(LevelEngine level)
        {
            var waveTimer = GetWaveTimer(level);
            // 每15帧检测一次，还剩一秒钟结束则不检测。
            if (!waveTimer.PassedInterval(15) || waveTimer.Frame <= 30)
                return;

            // 已经不存在存活的敌人了，直接加速。
            if (CountAliveEnemies(level) <= 0)
            {
                waveTimer.Frame = 30;
                return;
            }

            // 下一波是一大波，除非已经没有存活的敌人了，否则不会加速。
            if (level.IsHugeWave(level.CurrentWave + 1))
                return;

            // 还没到加速时间。
            if (waveTimer.Frame >= waveTimer.MaxFrame - GetWaveAdvanceTime(level))
                return;

            // 敌人的血量没有低于阈值，不加速。
            if (!CheckEnemiesRemainedHealth(level))
                return;

            // 加速。
            waveTimer.Frame = 30;
        }
        private void NextWaveOrHugeWave(LevelEngine level)
        {
            if (level.IsHugeWave(level.CurrentWave + 1))
            {
                TriggerHugeWaveApproaching(level);
                return;
            }
            NextWave(level);
        }
        private void TriggerHugeWaveApproaching(LevelEngine level)
        {
            level.WaveState = STATE_HUGE_WAVE_APPROACHING;
            var waveTimer = GetWaveTimer(level);
            waveTimer.MaxFrame = 180;
            waveTimer.Reset();
            BuiltinCallbacks.PostHugeWaveApproach.Run(level);
        }
        private void NextWave(LevelEngine level)
        {
            var waveTimer = GetWaveTimer(level);
            SetWaveMaxHealth(level, 0);
            level.NextWave();
            waveTimer.MaxFrame = GetWaveMaxTime(level);
            waveTimer.Reset();
            if (level.IsFinalWave(level.CurrentWave))
            {
                BuiltinCallbacks.PostFinalWave.Run(level);
                level.WaveState = STATE_FINAL_WAVE;
            }
        }
        #endregion

        #region 更新关卡
        protected virtual void NotStartedUpdate(LevelEngine level)
        {
            var waveTimer = GetWaveTimer(level);
            waveTimer.Run();
            if (waveTimer.Expired)
            {
                level.PlaySound(SoundID.awooga);
                level.WaveState = STATE_STARTED;
                level.LevelProgressVisible = true;
                NextWaveOrHugeWave(level);
            }
        }
        protected virtual void StartedUpdate(LevelEngine level)
        {
            var waveTimer = GetWaveTimer(level);
            waveTimer.Run();
            CheckWaveAdvancement(level);
            if (waveTimer.Expired)
            {
                NextWaveOrHugeWave(level);
            }
        }
        protected virtual void HugeWaveApproachingUpdate(LevelEngine level)
        {
            var waveTimer = GetWaveTimer(level);
            waveTimer.Run();
            if (waveTimer.Expired)
            {
                level.PlaySound(SoundID.siren);
                level.WaveState = STATE_STARTED;
                NextWave(level);
                level.SpawnEnemyAtRandomLane(level.ContentProvider.GetSpawnDefinition(EnemyID.flagZombie));
                level.RunHugeWaveEvent();
            }
        }
        protected virtual void FinalWaveUpdate(LevelEngine level)
        {
            var waveTimer = GetWaveTimer(level);
            waveTimer.Run();
            if (waveTimer.Expired)
            {
                level.SetNoProduction(true);
            }

            var lastEnemy = level.FindEntities(e => e.IsAliveEnemy()).FirstOrDefault();
            if (lastEnemy != null)
            {
                level.SetLastEnemy(new EntityID(lastEnemy));
            }
            else
            {
                level.SetNoProduction(true);
                if (!IsAllEnemiesCleared(level))
                {
                    SetAllEnemiesCleared(level, true);
                    var recorded = level.GetLastEnemy();
                    var enemy = recorded?.GetEntity(level);
                    Vector3 position;
                    if (enemy == null)
                    {
                        var x = level.GetProperty<float>(AreaProperties.ENEMY_SPAWN_X);
                        var z = level.GetEntityLaneZ(Mathf.CeilToInt(level.GetMaxLaneCount() * 0.5f));
                        var y = level.GetGroundY(x, z);
                        position = new Vector3(x, y, z);
                    }
                    else
                    {
                        position = enemy.Pos;
                    }
                    level.Produce(PickupID.clearPickup, position, enemy);
                }
            }
        }
        #endregion

        #region 敌人血量
        public float CountAliveEnemies(LevelEngine level)
        {
            return level.FindEntities(e => e.IsAliveEnemy()).Length;
        }
        public bool CheckEnemiesRemainedHealth(LevelEngine level)
        {
            var enemies = level.FindEntities(e => e.IsAliveEnemy());
            var health = enemies.Sum(e => e.Health + (e.EquipedArmor?.Health ?? 0));
            return health <= GetWaveAdvanceHealthPercent(level) * GetWaveMaxHealth(level);
        }
        #endregion


        #region 关卡属性
        public FrameTimer GetWaveTimer(LevelEngine level) => level.GetProperty<FrameTimer>("WaveTimer");
        public void SetWaveTimer(LevelEngine level, FrameTimer value) => level.SetProperty("WaveTimer", value);
        public int GetWaveMaxTime(LevelEngine level) => level.GetProperty<int>(BuiltinStageProps.WAVE_MAX_TIME);
        public int GetWaveAdvanceTime(LevelEngine level) => level.GetProperty<int>(BuiltinStageProps.WAVE_ADVANCE_TIME);

        public float GetWaveMaxHealth(LevelEngine level) => level.GetProperty<float>("WaveMaxHealth");
        public void SetWaveMaxHealth(LevelEngine level, float value) => level.SetProperty("WaveMaxHealth", value);
        public void AddWaveMaxHealth(LevelEngine level, float value) => SetWaveMaxHealth(level, GetWaveMaxHealth(level) + value);
        public float GetWaveAdvanceHealthPercent(LevelEngine level) => level.GetProperty<float>(BuiltinStageProps.WAVE_ADVANCE_HEALTH_PERCENT);

        public static bool IsAllEnemiesCleared(LevelEngine level)
        {
            return level.GetProperty<bool>(VanillaLevelProps.ALL_ENEMIES_CLEARED);
        }
        public static void SetAllEnemiesCleared(LevelEngine level, bool value)
        {
            level.SetProperty(VanillaLevelProps.ALL_ENEMIES_CLEARED, value);
        }
        #endregion

        #region 属性字段
        public const int STATE_NOT_STARTED = 0;
        public const int STATE_STARTED = 1;
        public const int STATE_HUGE_WAVE_APPROACHING = 2;
        public const int STATE_FINAL_WAVE = 3;
        #endregion
    }
}
