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
            Triggers.RunCallback(LevelCallbacks.POST_PREPARE_FOR_BATTLE, this);
        }
        public void RunFinalWaveEvent()
        {
            StageDefinition.PostFinalWaveEvent(this);
            Triggers.RunCallback(LevelCallbacks.POST_FINAL_WAVE_EVENT, this);
        }
        public void RunHugeWaveEvent()
        {
            StageDefinition.PostHugeWaveEvent(this);
            Triggers.RunCallback(LevelCallbacks.POST_HUGE_WAVE_EVENT, this);
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
        public void GameOver(int type, Entity killer, string message)
        {
            KillerEnemy = killer;
            OnGameOver?.Invoke(type, killer, message);
            Triggers.RunCallbackFiltered(LevelCallbacks.POST_GAME_OVER, type, type, killer, message);
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