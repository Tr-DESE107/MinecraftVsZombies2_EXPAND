using System;
using System.Collections.Generic;
using System.Linq;
using PVZEngine.Callbacks;
using PVZEngine.Definitions;
using PVZEngine.Entities;
using Tools;
using UnityEngine;

namespace PVZEngine.Level
{
    public partial class LevelEngine
    {
        public void PrepareForBattle()
        {
            StageDefinition.PrepareForBattle(this);
            LevelCallbacks.PostPrepareForBattle.Run(this);
        }
        public void RunFinalWaveEvent()
        {
            StageDefinition.PostFinalWaveEvent(this);
            LevelCallbacks.PostFinalWaveEvent.Run(this);
        }
        public void RunHugeWaveEvent()
        {
            StageDefinition.PostHugeWaveEvent(this);
            LevelCallbacks.PostHugeWaveEvent.Run(this);
        }
        public RandomGenerator GetSpawnRNG()
        {
            return spawnRandom;
        }
        public void AddSpawnedEnemyID(NamespaceID enemyId)
        {
            if (IsEnemySpawned(enemyId))
                return;
            spawnedID.Add(enemyId);
        }
        public bool RemoveSpawnedEnemyID(NamespaceID enemyId)
        {
            return spawnedID.Remove(enemyId);
        }
        public bool IsEnemySpawned(NamespaceID enemyId)
        {
            return spawnedID.Contains(enemyId);
        }
        public NamespaceID[] GetSpawnedEnemiesID()
        {
            return spawnedID.ToArray();
        }
        public int GetEnemySpawnX()
        {
            return GetProperty<int>(EngineAreaProps.ENEMY_SPAWN_X);
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
}