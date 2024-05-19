using System;
using System.Collections.Generic;
using UnityEngine;

namespace PVZEngine
{
    public partial class Game
    {
        #region 公有方法
        public Game(AreaDefinition area, StageDefinition stage)
        {
            AreaDefinition = area;
            StageDefinition = stage;

            // Initalize current stage info.
            var maxLane = GetMaxLaneCount();
            int maxColumn = GetMaxColumnCount();

            grids = new Grid[maxColumn * maxLane];

            var gridDefinitions = AreaDefinition.GetGrids();
            for (int i = 0; i < gridDefinitions.Length; i++)
            {
                var definition = gridDefinitions[i];
                int lane = Mathf.FloorToInt(i / maxColumn);
                int column = i % maxColumn;
                var grid = new Grid(this, definition, lane, column);
            }
        }

        #region 生命周期
        public void Init(int difficulty, GameOption option)
        {
            Seed = Guid.NewGuid().GetHashCode();

            Option = option;

            levelRandom = new RandomGenerator(Seed);
            entityRandom = new RandomGenerator(levelRandom.Next());
            effectRandom = new RandomGenerator(levelRandom.Next());
            roundRandom = new RandomGenerator(levelRandom.Next());
            spawnRandom = new RandomGenerator(levelRandom.Next());
            conveyorRandom = new RandomGenerator(levelRandom.Next());

            miscRandom = new RandomGenerator(levelRandom.Next());
#if UNITY_EDITOR
            debugRandom = new RandomGenerator(levelRandom.Next());
#endif

            Difficulty = difficulty;
            Energy = option.StartEnergy;
        }
        public void Update()
        {
            UpdateSeedRecharges();
            foreach (var entity in GetEntities())
            {
                entity.Update();
            }
        }
        #endregion

        #region 原版属性
        public bool IsAutoCollect()
        {
            return GetProperty<bool>(GameProperties.AUTO_COLLECT);
        }
        public bool IsNoProduction()
        {
            return GetProperty<bool>(GameProperties.NO_PRODUCTION);
        }
        #endregion

        #region 属性
        public void SetProperty(string name, bool value)
        {
            propertyDict[name] = value;
        }
        public object GetProperty(string name, bool ignoreStageDefinition = false, bool ignoreAreaDefinition = false)
        {
            object result = null;
            if (propertyDict.TryGetValue(name, out var value))
                result = value;
            else if (!ignoreStageDefinition)
                result = StageDefinition.GetProperty<object>(name);
            else if (!ignoreAreaDefinition)
                result = AreaDefinition.GetProperty<object>(name);
            return result;
        }
        public T GetProperty<T>(string name, bool ignoreStageDefinition = false, bool ignoreAreaDefinition = false)
        {
            if (GetProperty(name, ignoreStageDefinition, ignoreAreaDefinition) is T tProp)
                return tProp;
            return default;
        }
        #endregion

        #region 坐标相关方法
        public float GetTopZOffset()
        {
            return TOP_Z_OFFSET + AreaDefinition.GetProperty<float>(AreaProperties.TOP_Z_OFFSET);
        }
        public int GetMaxLaneCount()
        {
            return AreaDefinition.GetProperty<int>(AreaProperties.MAX_LANE_COUNT);
        }
        public int GetMaxColumnCount()
        {
            return AreaDefinition.GetProperty<int>(AreaProperties.MAX_COLUMN_COUNT);
        }
        public int GetLane(float z)
        {
            //4.0 - 4.8 : 0
            //3.2 - 4.0 : 1
            //2.4 - 3.2 : 2
            //1.6 - 2.4 : 3
            //0.8 - 1.6 : 4
            return Mathf.FloorToInt((SCREEN_HEIGHT - GetTopZOffset() - z) / GRID_SIZE);
        }
        public int GetColumn(float x)
        {
            //2.4 - 3.2 : 0
            //3.2 - 4.0 : 1
            //4.0 - 4.8 : 2
            //5.6 - 6.2 : 3
            return Mathf.FloorToInt((x - GRID_SIZE * 0.5f - LEFT_BORDER) / GRID_SIZE);
        }
        public float GetGroundHeight(Vector3 pos)
        {
            return GetGroundHeight(pos.x, pos.z);
        }
        public float GetGroundHeight(float x, float z)
        {
            return 0;
        }
        public Grid GetGrid(int column, int lane)
        {
            if (column < 0 || column >= GetMaxColumnCount() || lane < 0 || lane > GetMaxLaneCount())
            {
                return null;
            }
            else
            {
                return grids[lane * GetMaxColumnCount() + column];
            }
        }

        public Grid GetGrid(Vector2Int pos)
        {
            return GetGrid(pos.x, pos.y);
        }
        #endregion 坐标相关方法

        #region 随机数
        public int GetSpawnRandomRange(int min, int max) => spawnRandom.Next(min, max);
        public int GetRoundEnemyRandom() => roundRandom.Next();
        public int GetRandomLane()
        {
            return GetSpawnRandomRange(1, GetMaxLaneCount() + 1);
        }
        #endregion

        #region 时间
        public int GetSecondTicks(float second)
        {
            return Mathf.CeilToInt(second * TPS);
        }
        #endregion

        #endregion

        #region 属性字段
        public int Seed { get; private set; }
        public bool IsCleared { get; private set; }
        public StageDefinition StageDefinition { get; private set; }
        public AreaDefinition AreaDefinition { get; private set; }
        /// <summary>
        /// 进屋的僵尸。
        /// </summary>
        public Enemy KillerEnemy { get; private set; }
        public bool IsEndless { get; set; }
        /// <summary>
        /// 游戏是否已经通关过一次。
        /// </summary>
        public bool IsRerun { get; set; }
        public int Difficulty { get; set; }
        public int TPS => Option.TPS;
        public GameOption Option { get; private set; }
        private RandomGenerator levelRandom;

        private RandomGenerator entityRandom;
        private RandomGenerator effectRandom;

        private RandomGenerator roundRandom;
        private RandomGenerator spawnRandom;
        private RandomGenerator conveyorRandom;

        private RandomGenerator debugRandom;
        private RandomGenerator miscRandom;

        private string deathMessage;

        private Dictionary<string, object> propertyDict = new Dictionary<string, object>();
        private Grid[] grids;

        #endregion 保存属性
    }
}