using System;
using System.Collections.Generic;
using System.Linq;
using log4net.Core;
using MVZ2.GameContent.Enemies;
using MVZ2.Vanilla;
using PVZEngine;
using UnityEngine;

namespace MVZ2.GameContent.Stages
{
    public class ClassicStage : StageDefinition
    {
        public ClassicStage(string nsp, string name, int totalFlags, EnemySpawnEntry[] spawnEntries) : base(nsp, name)
        {
            SetProperty(StageProps.WAVE_MAX_TIME, 900);
            SetProperty(StageProps.WAVE_ADVANCE_TIME, 300);
            SetProperty(StageProps.WAVE_ADVANCE_HEALTH_PERCENT, 0.6f);
            SetProperty(StageProperties.TOTAL_FLAGS, totalFlags);
            this.spawnEntries = spawnEntries;
        }
        public override void Start(Game level)
        {
            var time = level.CurrentFlag > 0 ? level.GetContinutedFirstWaveTime() : level.GetFirstWaveTime();
            var waveTimer = new FrameTimer(time);
            SetWaveTimer(level, waveTimer);
        }
        public override void Update(Game level)
        {
            if (level.CurrentWave >= level.GetTotalWaveCount())
                return;
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
            }
        }

        public override void PostEnemySpawned(Entity entity)
        {
            base.PostEnemySpawned(entity);
            AddWaveMaxHealth(entity.Game, entity.GetMaxHealth() + (entity.EquipedArmor?.GetMaxHealth() ?? 0));
        }

        public float CountAliveEnemies(Game level)
        {
            return level.FindEntities(e => IsAliveEnemy(e)).Length;
        }
        public bool CheckEnemiesRemainedHealth(Game level)
        {
            var enemies = level.FindEntities(e => IsAliveEnemy(e));
            var health = enemies.Sum(e => e.Health + (e.EquipedArmor?.Health ?? 0));
            return health <= GetWaveAdvanceHealthPercent(level) * GetWaveMaxHealth(level);
        }
        public void CreatePreviewEnemies(Game level, IList<NamespaceID> validEnemies, Rect region)
        {
            List<Entity> createdEnemies = new List<Entity>();

            int loopTimes = 0;

            while (true)
            {
                for (int i = 0; i < validEnemies.Count; i++)
                {
                    var entityRef = validEnemies[i];

                    int times = 1;
                    for (int time = 0; time < times; time++)
                    {
                        bool around;
                        Vector3 pos;
                        do
                        {
                            pos = new Vector3(UnityEngine.Random.Range(region.xMin, region.xMax), 0, UnityEngine.Random.Range(region.yMin, region.yMax));

                            around = false;
                            for (int e = 0; e < createdEnemies.Count; e++)
                            {
                                Vector3 createdPos = createdEnemies[e].Pos;
                                if (Vector3.Distance(createdPos, pos) < 80)
                                {
                                    around = true;
                                    break;
                                }
                            }
                        }
                        while (around);

                        Entity enm = level.Spawn(entityRef, pos, null);
                        enm.SetPreviewEnemy(true);
                        createdEnemies.Add(enm);

                        if (createdEnemies.Count >= Mathf.Max(6, validEnemies.Count + 3))
                        {
                            return;
                        }
                    }
                }

                loopTimes++;
                if (loopTimes > 1024)
                {
                    Debug.Log("次数超过上限，跳出循环。");
                    return;
                }
            }
        }
        public override IEnumerable<IEnemySpawnEntry> GetEnemyPool()
        {
            return spawnEntries;
        }
        public FrameTimer GetWaveTimer(Game level) => level.GetProperty<FrameTimer>("WaveTimer");
        public void SetWaveTimer(Game level, FrameTimer value) => level.SetProperty("WaveTimer", value);
        public int GetWaveMaxTime(Game level) => level.GetProperty<int>(StageProps.WAVE_MAX_TIME);
        public int GetWaveAdvanceTime(Game level) => level.GetProperty<int>(StageProps.WAVE_ADVANCE_TIME);

        public float GetWaveMaxHealth(Game level) => level.GetProperty<float>("WaveMaxHealth");
        public void SetWaveMaxHealth(Game level, float value) => level.SetProperty("WaveMaxHealth", value);
        public void AddWaveMaxHealth(Game level, float value) => SetWaveMaxHealth(level, GetWaveMaxHealth(level) + value);
        public float GetWaveAdvanceHealthPercent(Game level) => level.GetProperty<float>(StageProps.WAVE_ADVANCE_HEALTH_PERCENT);
        protected virtual void NotStartedUpdate(Game level)
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
        protected virtual void StartedUpdate(Game level)
        {
            var waveTimer = GetWaveTimer(level);
            waveTimer.Run();
            if (!level.IsHugeWave(level.CurrentWave + 1) && waveTimer.Frame % 30 == 0 && !waveTimer.Expired)
            {
                if (waveTimer.Frame < waveTimer.MaxFrame - GetWaveAdvanceTime(level) || CountAliveEnemies(level) <= 0)
                {
                    if (CheckEnemiesRemainedHealth(level))
                    {
                        waveTimer.Frame = 29;
                    }
                }
            }
            if (waveTimer.Expired)
            {
                NextWaveOrHugeWave(level);
            }
        }
        protected virtual void HugeWaveApproachingUpdate(Game level)
        {
            var waveTimer = GetWaveTimer(level);
            waveTimer.Run();
            if (waveTimer.Expired)
            {
                level.PlaySound(SoundID.siren);
                level.WaveState = STATE_STARTED;
                NextWave(level);
                level.RunHugeWaveEvent();
            }
        }
        private bool IsAliveEnemy(Entity entity)
        {
            return entity.Type == EntityTypes.ENEMY && !entity.GetProperty<bool>(EnemyProps.HARMLESS) && entity.IsEnemy(entity.Game.Option.LeftFaction);
        }
        private void NextWaveOrHugeWave(Game level)
        {
            if (level.IsHugeWave(level.CurrentWave + 1))
            {
                TriggerHugeWaveApproaching(level);
                return;
            }
            NextWave(level);
        }
        private void TriggerHugeWaveApproaching(Game level)
        {
            level.WaveState = STATE_HUGE_WAVE_APPROACHING;
            var waveTimer = GetWaveTimer(level);
            waveTimer.MaxFrame = 300;
            waveTimer.Reset();
            level.PlaySound(SoundID.hugeWave);
        }
        private void NextWave(Game level)
        {
            var waveTimer = GetWaveTimer(level);
            SetWaveMaxHealth(level, 0);
            level.NextWave();
            waveTimer.MaxFrame = GetWaveMaxTime(level);
            waveTimer.Reset();
            if (level.IsFinalWave(level.CurrentWave))
            {
                level.PlaySound(SoundID.finalWave);
            }
        }
        public const int STATE_NOT_STARTED = 0;
        public const int STATE_STARTED = 1;
        public const int STATE_HUGE_WAVE_APPROACHING = 2;
        private EnemySpawnEntry[] spawnEntries;

    }
    [Serializable]
    public class EnemySpawnEntry : IEnemySpawnEntry
    {
        public NamespaceID spawnRef;
        public int earliestFlag;
        public EnemySpawnEntry(NamespaceID spawnRef, int earliestFlag = 0)
        {
            this.spawnRef = spawnRef;
            this.earliestFlag = earliestFlag;
        }

        public bool CanSpawn(Game game)
        {
            return game.CurrentFlag >= earliestFlag;
        }

        public SpawnDefinition GetSpawnDefinition(Game game)
        {
            return game.GetSpawnDefinition(spawnRef);
        }
    }
}
