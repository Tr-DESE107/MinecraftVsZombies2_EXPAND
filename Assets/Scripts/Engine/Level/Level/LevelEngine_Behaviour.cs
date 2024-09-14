using System;
using System.Collections.Generic;
using System.Linq;
using PVZEngine.Definitions;
using Tools;
using UnityEngine;

namespace PVZEngine.Level
{
    public partial class LevelEngine
    {
        public void NextWave()
        {
            if (IsHugeWave(CurrentWave))
            {
                CurrentFlag++;
            }
            CurrentWave++;
            SpawnWaveEnemies(CurrentWave);
            PostWave(CurrentWave);
        }
        public void PrepareForBattle()
        {
            StageDefinition.PrepareForBattle(this);
            LevelCallbacks.PostPrepareForBattle.Run(this);
        }
        public void SpawnWaveEnemies(int wave)
        {
            var totalEnergy = Mathf.Ceil(wave / 3f);
            if (IsHugeWave(wave))
            {
                totalEnergy *= 2.5f;
            }
            var pool = GetEnemyPool();
            var spawnDefs = pool.Where(e => e.CanSpawn(this)).Select(e => e.GetSpawnDefinition(ContentProvider));
            while (totalEnergy > 0)
            {
                var validSpawnDefs = spawnDefs.Where(def => def.SpawnCost > 0 && def.SpawnCost <= totalEnergy);
                if (validSpawnDefs.Count() <= 0)
                    break;
                var spawnDef = validSpawnDefs.Random(spawnRandom);
                SpawnEnemyAtRandomLane(spawnDef);
                totalEnergy -= spawnDef.SpawnCost;
            }

            if (IsFinalWave(wave))
            {
                var poolSpawnDefs = pool.Select(e => e.GetSpawnDefinition(ContentProvider));
                var notSpawnedDefs = poolSpawnDefs.Where(def => !spawnedID.Contains(def.GetID()));
                foreach (var notSpawnedDef in notSpawnedDefs)
                {
                    SpawnEnemyAtRandomLane(notSpawnedDef);
                }
            }
        }
        public void RunFinalWaveEvent()
        {
            StageDefinition.PostFinalWave(this);
            LevelCallbacks.PostFinalWave.Run(this);
        }
        public void RunHugeWaveEvent()
        {
            StageDefinition.PostHugeWave(this);
            LevelCallbacks.PostHugeWave.Run(this);
        }
        public bool IsHugeWave(int wave)
        {
            return wave > 0 && wave % GetWavesPerFlag() == 0;
        }
        public bool IsFinalWave(int wave)
        {
            return wave == GetTotalWaveCount();
        }
        public int GetTotalWaveCount()
        {
            return GetTotalFlags() * GetWavesPerFlag();
        }
        public int GetEnemySpawnX()
        {
            return GetProperty<int>(AreaProperties.ENEMY_SPAWN_X);
        }
        public int GetTotalFlags()
        {
            return GetProperty<int>(StageProperties.TOTAL_FLAGS);
        }
        public int GetWavesPerFlag()
        {
            return GetProperty<int>(StageProperties.WAVES_PER_FLAG);
        }
        public int GetFirstWaveTime()
        {
            return GetProperty<int>(StageProperties.FIRST_WAVE_TIME);
        }
        public int GetContinutedFirstWaveTime()
        {
            return GetProperty<int>(StageProperties.CONTINUED_FIRST_WAVE_TIME);
        }
        public IEnumerable<IEnemySpawnEntry> GetEnemyPool()
        {
            return StageDefinition.GetEnemyPool();
        }
        public bool WillEnemySpawn(NamespaceID spawnRef)
        {
            var pool = GetEnemyPool();
            if (pool == null)
                return false;
            return pool.Any(e => e.GetSpawnDefinition(ContentProvider).GetID() == spawnRef);
        }
        public Entity SpawnEnemyAtRandomLane(SpawnDefinition spawnDef)
        {
            if (spawnDef == null)
                return null;
            var lane = GetRandomEnemySpawnLane();
            return SpawnEnemy(spawnDef, lane);
        }
        public Entity SpawnEnemy(SpawnDefinition spawnDef, int lane)
        {
            if (spawnDef == null)
                return null;
            var x = GetEnemySpawnX();
            var z = GetEntityLaneZ(lane);
            var y = GetGroundY(x, z);
            var pos = new Vector3(x, y, z);
            var enemy = Spawn(spawnDef.EntityID, pos, null);
            spawnedID.Add(spawnDef.GetID());
            PostEnemySpawned(enemy);
            return enemy;
        }
        public void GameOver(int type, Entity killer, string message)
        {
            OnGameOver?.Invoke(type, killer, message);
        }
        private void PostWave(int wave)
        {
            StageDefinition.PostWave(this, wave);
            LevelCallbacks.PostWave.Run(this, wave);
        }
        private void PostEnemySpawned(Entity enemy)
        {
            StageDefinition.PostEnemySpawned(enemy);
            LevelCallbacks.PostEnemySpawned.Run(enemy);
        }
        public int GetRandomEnemySpawnLane()
        {
            var length = GetMaxLaneCount();
            if (spawnedLanes.Count >= length)
            {
                spawnedLanes.Clear();
            }

            var pool = new List<int>();
            for (int i = 0; i < length; i++)
            {
                if (spawnedLanes.Contains(i))
                    continue;
                pool.Add(i);
            }
            int row = pool.Random(spawnRandom);

            spawnedLanes.Add(row);
            return row;
        }
        public event Action<int, Entity, string> OnGameOver;
        public int CurrentWave { get; set; }
        public int CurrentFlag { get; set; }
        public int WaveState { get; set; }
        public bool LevelProgressVisible { get; set; }
        private List<int> spawnedLanes = new List<int>();
        private List<NamespaceID> spawnedID = new List<NamespaceID>();
    }
    public interface IEnemySpawnEntry
    {
        bool CanSpawn(LevelEngine game);
        SpawnDefinition GetSpawnDefinition(IContentProvider game);
    }
}