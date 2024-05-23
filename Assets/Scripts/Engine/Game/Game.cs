using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PVZEngine
{
    public partial class Game
    {
        #region 公有方法
        public Game(params Mod[] mods)
        {
            foreach (var mod in mods)
            {
                AddMod(mod);
            }
        }

        #region 生命周期
        public void Init(int difficulty, NamespaceID areaId, NamespaceID stageId, GameOption option, int seed = 0)
        {
            Seed = seed == 0 ? Guid.NewGuid().GetHashCode() : seed;

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


            AreaDefinition = GetAreaDefinition(areaId);
            StageDefinition = GetStageDefinition(stageId);

            gridSize = AreaDefinition.GetProperty<float>(AreaProperties.GRID_SIZE);
            gridLeftX = AreaDefinition.GetProperty<float>(AreaProperties.GRID_LEFT_X);
            gridBottomZ = AreaDefinition.GetProperty<float>(AreaProperties.GRID_BOTTOM_Z);
            maxLaneCount = AreaDefinition.GetProperty<int>(AreaProperties.MAX_LANE_COUNT);
            maxColumnCount = AreaDefinition.GetProperty<int>(AreaProperties.MAX_COLUMN_COUNT);

            // Initalize current stage info.
            var maxLane = GetMaxLaneCount();
            int maxColumn = GetMaxColumnCount();

            grids = new Grid[maxColumn * maxLane];

            var gridDefinitions = AreaDefinition.GetGridDefintionsID().Select(i => GetGridDefinition(i)).ToArray();
            for (int i = 0; i < gridDefinitions.Length; i++)
            {
                var definition = gridDefinitions[i];
                int lane = Mathf.FloorToInt(i / maxColumn);
                int column = i % maxColumn;
                grids[i] = new Grid(this, definition, lane, column);
            }
        }
        public void Update()
        {
            UpdateSeedRecharges();
            var entities = GetEntities();
            foreach (var entity in entities)
            {
                entity.Update();
                CollisionUpdate(entity, entities);
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
            return PropertyDictionary.ToGeneric<T>(GetProperty(name, ignoreStageDefinition, ignoreAreaDefinition));
        }
        #endregion

        #region 坐标相关方法
        public int GetGridIndex(int column, int lane)
        {
            return column + lane * GetMaxColumnCount();
        }
        public float GetGridSize()
        {
            return gridSize;
        }
        public float GetGridRightX()
        {
            return GetGridLeftX() + GetMaxColumnCount() * GetGridSize();
        }
        public float GetGridLeftX()
        {
            return gridLeftX;
        }
        public float GetGridTopZ()
        {
            return GetGridBottomZ() + GetMaxLaneCount() * GetGridSize();
        }
        public float GetGridBottomZ()
        {
            return gridBottomZ;
        }
        public int GetMaxLaneCount()
        {
            return maxLaneCount;
        }
        public int GetMaxColumnCount()
        {
            return maxColumnCount;
        }
        public int GetLane(float z)
        {
            //4.0 - 4.8 : 0
            //3.2 - 4.0 : 1
            //2.4 - 3.2 : 2
            //1.6 - 2.4 : 3
            //0.8 - 1.6 : 4
            return Mathf.FloorToInt((GetGridTopZ() - z) / GetGridSize());
        }
        public int GetColumn(float x)
        {
            //2.4 - 3.2 : 0
            //3.2 - 4.0 : 1
            //4.0 - 4.8 : 2
            //5.6 - 6.2 : 3
            return Mathf.FloorToInt((x - GetGridLeftX()) / GetGridSize());
        }
        public float GetEntityLaneZ(int row)
        {
            return GetLaneZ(row) + 16;
        }
        public float GetEntityColumnX(int column)
        {
            return GetColumnX(column) + GetGridSize() * 0.5f;
        }
        public float GetColumnX(int column)
        {
            return GetGridLeftX() + column * GetGridSize();
        }
        public float GetLaneZ(int lane)
        {
            return GetGridTopZ() - (lane + 1) * GetGridSize();
        }
        public float GetGroundHeight(Vector3 pos)
        {
            return GetGroundHeight(pos.x, pos.z);
        }
        public float GetGroundHeight(float x, float z)
        {
            return 0;
        }
        public Grid GetGrid(int index)
        {
            if (index < 0 || index >= GetMaxColumnCount() * GetMaxLaneCount())
                return null;
            return grids[index];
        }

        public Grid GetGrid(int column, int lane)
        {
            if (column < 0 || column >= GetMaxColumnCount() || lane < 0 || lane >= GetMaxLaneCount())
                return null;
            return GetGrid(lane * GetMaxColumnCount() + column);
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

        public Buff CreateBuff<T>() where T :BuffDefinition
        {
            var buffDefinition = GetBuffDefinition<T>();
            if (buffDefinition == null)
                return null;
            return new Buff(buffDefinition);
        }
        public Buff CreateBuff(NamespaceID id)
        {
            var buffDefinition = GetBuffDefinition(id);
            if (buffDefinition == null)
                return null;
            return new Buff(buffDefinition);
        }

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

        private float gridSize;
        private float gridLeftX;
        private float gridBottomZ;
        private int maxLaneCount;
        private int maxColumnCount;
        #endregion 保存属性
    }
}