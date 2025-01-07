using System.Linq;
using MVZ2.GameContent.Difficulties;
using MVZ2.GameContent.Enemies;
using MVZ2.GameContent.Pickups;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Callbacks;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Level;
using MVZ2Logic.Level;
using PVZEngine;
using PVZEngine.Armors;
using PVZEngine.Definitions;
using PVZEngine.Entities;
using PVZEngine.Level;
using Tools;
using UnityEngine;

namespace MVZ2.GameContent.Stages
{
    public abstract class WaveStageBehaviourBase : StageBehaviour
    {
        public WaveStageBehaviourBase(StageDefinition stageDef) : base(stageDef)
        {
            stageDef.SetProperty(EngineStageProps.CONTINUED_FIRST_WAVE_TIME, 180);
            stageDef.SetProperty(VanillaStageProps.WAVE_MAX_TIME, 900);
            stageDef.SetProperty(VanillaStageProps.WAVE_ADVANCE_TIME, 300);
            stageDef.SetProperty(VanillaStageProps.WAVE_ADVANCE_HEALTH_PERCENT, 0.6f);
        }
        #region 回调
        public override void PrepareForBattle(LevelEngine level)
        {
            level.AreaDefinition.PrepareForBattle(level);
            if (level.Difficulty != VanillaDifficulties.hard && level.CurrentFlag <= 0)
            {
                var cartRef = level.GetProperty<NamespaceID>(EngineAreaProps.CART_REFERENCE);
                level.SpawnCarts(cartRef, VanillaLevelExt.CART_START_X, 20);
            }
        }
        public override void Start(LevelEngine level)
        {
            var time = level.CurrentFlag > 0 ? level.GetContinutedFirstWaveTime() : level.GetFirstWaveTime();
            var waveTimer = new FrameTimer(time);
            SetWaveTimer(level, waveTimer);
            level.WaveState = STATE_NOT_STARTED;
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
        public override void PostHugeWaveEvent(LevelEngine level)
        {
            base.PostHugeWaveEvent(level);
            if (level.IgnoreHugeWaveEvent())
                return;
            level.AreaDefinition.PostHugeWaveEvent(level);
        }
        public override void PostFinalWaveEvent(LevelEngine level)
        {
            base.PostHugeWaveEvent(level);
            level.AreaDefinition.PostFinalWaveEvent(level);
        }
        public override void PostEnemySpawned(Entity entity)
        {
            AddWaveMaxHealth(entity.Level, entity.GetMaxHealth() + (entity.EquipedArmor?.GetMaxHealth() ?? 0));
        }
        #endregion

        #region 波次
        protected void CheckWaveAdvancement(LevelEngine level)
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
            if (waveTimer.Frame >= waveTimer.MaxFrame - level.GetWaveAdvanceTime())
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
            waveTimer.ResetTime(180);
            level.Triggers.RunCallback(VanillaCallbacks.POST_HUGE_WAVE_APPROACH, level);
        }
        private void NextWave(LevelEngine level)
        {
            var waveTimer = GetWaveTimer(level);
            waveTimer.ResetTime(level.GetWaveMaxTime());
            SetWaveMaxHealth(level, 0);
            level.NextWave();
            if (level.IsFinalWave(level.CurrentWave))
            {
                StartFinalWave(level);
            }
        }
        private void StartFinalWave(LevelEngine level)
        {
            level.WaveState = STATE_FINAL_WAVE;
            if (ShouldTriggerFinalWaveEvent(level))
            {
                SetFinalWaveEventTimer(level, new FrameTimer(60));
                level.Triggers.RunCallback(VanillaCallbacks.POST_FINAL_WAVE, level);
            }
        }
        protected virtual bool ShouldTriggerFinalWaveEvent(LevelEngine level)
        {
            return true;
        }
        #endregion

        #region 更新关卡
        protected virtual void NotStartedUpdate(LevelEngine level)
        {
            var waveTimer = GetWaveTimer(level);
            waveTimer.Run();
            if (waveTimer.Expired)
            {
                level.PlaySound(VanillaSoundID.awooga);
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
                level.PlaySound(VanillaSoundID.siren);
                level.WaveState = STATE_STARTED;
                NextWave(level);
                if (SpawnFlagZombie)
                {
                    level.SpawnEnemyAtRandomLane(level.Content.GetSpawnDefinition(VanillaSpawnID.flagZombie));
                }
                level.RunHugeWaveEvent();
            }
        }
        protected virtual void FinalWaveUpdate(LevelEngine level)
        {
            var finalWaveTimer = GetFinalWaveEventTimer(level);
            if (finalWaveTimer != null)
            {
                finalWaveTimer.Run();
                if (finalWaveTimer.Expired)
                {
                    level.RunFinalWaveEvent();
                }
            }
        }
        protected virtual void CheckClearUpdate(LevelEngine level)
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
                level.SetLastEnemyPosition(lastEnemy.Position);
            }
            else
            {
                level.PostWaveFinished(level.CurrentWave);
                level.SetNoProduction(true);
                if (!level.IsAllEnemiesCleared())
                {
                    level.SetAllEnemiesCleared(true);
                    var lastEnemyPosition = level.GetLastEnemyPosition();
                    Vector3 position;
                    if (lastEnemyPosition.x <= VanillaLevelExt.GetBorderX(false))
                    {
                        var x = level.GetEnemySpawnX();
                        var z = level.GetEntityLaneZ(Mathf.CeilToInt(level.GetMaxLaneCount() * 0.5f));
                        var y = level.GetGroundY(x, z);
                        position = new Vector3(x, y, z);
                    }
                    else
                    {
                        position = lastEnemyPosition;
                    }
                    level.Produce(VanillaPickupID.clearPickup, position, null);
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
            return health <= level.GetWaveAdvanceHealthPercent() * GetWaveMaxHealth(level);
        }
        #endregion

        #region 关卡属性
        public FrameTimer GetWaveTimer(LevelEngine level) => level.GetProperty<FrameTimer>("WaveTimer");
        public void SetWaveTimer(LevelEngine level, FrameTimer value) => level.SetProperty("WaveTimer", value);

        public FrameTimer GetFinalWaveEventTimer(LevelEngine level) => level.GetProperty<FrameTimer>("FinalWaveEventTimer");
        public void SetFinalWaveEventTimer(LevelEngine level, FrameTimer value) => level.SetProperty("FinalWaveEventTimer", value);

        public float GetWaveMaxHealth(LevelEngine level) => level.GetProperty<float>("WaveMaxHealth");
        public void SetWaveMaxHealth(LevelEngine level, float value) => level.SetProperty("WaveMaxHealth", value);
        public void AddWaveMaxHealth(LevelEngine level, float value) => SetWaveMaxHealth(level, GetWaveMaxHealth(level) + value);
        #endregion

        #region 属性字段
        public bool SpawnFlagZombie { get; set; } = true;
        public const int STATE_NOT_STARTED = 0;
        public const int STATE_STARTED = 1;
        public const int STATE_HUGE_WAVE_APPROACHING = 2;
        public const int STATE_FINAL_WAVE = 3;
        public const int STATE_AFTER_FINAL_WAVE = 4;
        public const int STATE_BOSS_FIGHT = 5;
        public const int STATE_AFTER_BOSS = 6;
        #endregion
    }
}
