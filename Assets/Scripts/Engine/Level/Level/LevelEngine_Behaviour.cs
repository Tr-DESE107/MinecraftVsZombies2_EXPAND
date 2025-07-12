using System;
using System.Collections.Generic;
using System.Linq;
using PVZEngine.Callbacks;
using PVZEngine.Definitions;
using PVZEngine.Entities;
using Tools;

namespace PVZEngine.Level
{
    public partial class LevelEngine
    {
        public void PrepareForBattle()
        {
            StageDefinition.PrepareForBattle(this);
            var param = new LevelCallbackParams(this);
            Triggers.RunCallback(LevelCallbacks.POST_PREPARE_FOR_BATTLE, param);
        }
        public void RunFinalWaveEvent()
        {
            StageDefinition.PostFinalWaveEvent(this);
            var param = new LevelCallbackParams(this);
            Triggers.RunCallback(LevelCallbacks.POST_FINAL_WAVE_EVENT, param);
        }
        public void RunHugeWaveEvent()
        {
            StageDefinition.PostHugeWaveEvent(this);
            var param = new LevelCallbackParams(this);
            Triggers.RunCallback(LevelCallbacks.POST_HUGE_WAVE_EVENT, param);
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
        public float GetEnemySpawnX()
        {
            return GetProperty<float>(EngineAreaProps.ENEMY_SPAWN_X);
        }
        public void GameOver(int type, Entity killer, string message)
        {
            KillerEnemy = killer;
            OnGameOver?.Invoke(type, killer, message);
            var param = new LevelCallbacks.PostGameOverParams()
            {
                level = this,
                type = type,
                killer = killer,
                message = message
            };
            Triggers.RunCallbackFiltered(LevelCallbacks.POST_GAME_OVER, param, type);
        }
        public int GetRandomEnemySpawnLane()
        {
            var length = GetMaxLaneCount();
            return GetRandomEnemySpawnLane(Enumerable.Range(0, length));
        }
        public int GetRandomEnemySpawnLane(IEnumerable<int> lanes)
        {
            if (lanes.Count() <= 0)
                return -1;
            var possibleLanes = lanes.Where(l => !spawnedLanes.Contains(l));
            if (possibleLanes.Count() <= 0)
            {
                spawnedLanes.Clear();
                possibleLanes = lanes;
            }
            int row = possibleLanes.Random(GetSpawnRNG());
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