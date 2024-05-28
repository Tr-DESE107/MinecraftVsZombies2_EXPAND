using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PVZEngine
{
    public partial class Level
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
        public void SpawnWaveEnemies(int wave)
        {
            var totalEnergy = Mathf.Ceil(wave / 3f);
            if (IsHugeWave(wave))
            {
                totalEnergy *= 2.5f;
            }
            var pool = GetEnemyPool();
            var spawnDefs = pool.Where(e => e.CanSpawn(this)).Select(e => e.GetSpawnDefinition(this));
            while (totalEnergy > 0)
            {
                var validSpawnDefs = spawnDefs.Where(def => def.SpawnCost <= totalEnergy);
                if (validSpawnDefs.Count() <= 0)
                    break;
                var spawnDef = validSpawnDefs.Random(spawnRandom);
                SpawnEnemy(spawnDef);
                totalEnergy -= spawnDef.SpawnCost;
            }

            if (IsFinalWave(wave))
            {
                var poolSpawnDefs = pool.Select(e => e.GetSpawnDefinition(this));
                var notSpawnedDefs = poolSpawnDefs.Where(def => !spawnedID.Contains(def.GetID()));
                foreach (var notSpawnedDef in notSpawnedDefs)
                {
                    SpawnEnemy(notSpawnedDef);
                }
            }
        }
        public void RunFinalWaveEvent()
        {
            StageDefinition.PostFinalWave(this);
            Callbacks.PostFinalWave.Run(this);
        }
        public void RunHugeWaveEvent()
        {
            StageDefinition.PostHugeWave(this);
            Callbacks.PostHugeWave.Run(this);
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
            return pool.Any(e => e.GetSpawnDefinition(this).GetID() == spawnRef);
        }
        private void PostWave(int wave)
        {
            StageDefinition.PostWave(this, wave);
            Callbacks.PostWave.Run(this, wave);
        }
        private void PostEnemySpawned(Entity enemy)
        {
            StageDefinition.PostEnemySpawned(enemy);
            Callbacks.PostEnemySpawned.Run(enemy);
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
        private Entity SpawnEnemy(SpawnDefinition spawnDef)
        {
            var lane = GetRandomEnemySpawnLane();
            var x = GetEnemySpawnX();
            var z = GetEntityLaneZ(lane);
            var y = GetGroundY(x, z);
            var pos = new Vector3(x, y, z);
            var enemy = Spawn(spawnDef.EntityID, pos, null);
            spawnedID.Add(spawnDef.GetID());
            PostEnemySpawned(enemy);
            return enemy;
        }
        public int CurrentWave { get; set; }
        public int CurrentFlag { get; set; }
        public int WaveState { get; set; }
        public bool LevelProgressVisible { get; set; }
        private List<int> spawnedLanes = new List<int>();
        private List<NamespaceID> spawnedID = new List<NamespaceID>();
    }
    public interface IEnemySpawnEntry
    {
        bool CanSpawn(Level game);
        SpawnDefinition GetSpawnDefinition(Level game);
    }
}