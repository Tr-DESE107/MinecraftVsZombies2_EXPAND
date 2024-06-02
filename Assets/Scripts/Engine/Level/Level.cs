using System;
using System.Collections.Generic;
using System.Linq;
using PVZEngine.Serialization;
using UnityEngine;

namespace PVZEngine
{
    public partial class Level
    {
        #region 公有方法
        public Level(params Mod[] mods)
        {
            foreach (var mod in mods)
            {
                AddMod(mod);
            }
        }

        #region 生命周期
        public void Init(NamespaceID areaId, NamespaceID stageId, LevelOption option, int seed = 0)
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

            Energy = option.StartEnergy;


            AreaDefinition = GetAreaDefinition(areaId);
            StageDefinition = GetStageDefinition(stageId);

            InitAreaProperties();

            // Initalize current stage info.
            var maxLane = GetMaxLaneCount();
            int maxColumn = GetMaxColumnCount();

            grids = new LawnGrid[maxColumn * maxLane];

            var gridDefinitions = AreaDefinition.GetGridDefintionsID().Select(i => GetGridDefinition(i)).ToArray();
            for (int i = 0; i < gridDefinitions.Length; i++)
            {
                var definition = gridDefinitions[i];
                int lane = Mathf.FloorToInt(i / maxColumn);
                int column = i % maxColumn;
                grids[i] = new LawnGrid(this, definition, lane, column);
            }
        }
        public void Start(int difficulty)
        {
            Difficulty = difficulty;
            StageDefinition.Start(this);
            Callbacks.PostLevelStart.Run(this);
        }
        public void Update()
        {
            UpdateSeedRecharges();
            collisionCachedBounds.Clear();
            var entities = GetEntities();
            foreach (var entity in entities)
            {
                entity.Update();
                CollisionUpdate(entity, entity.CollisionMask, entities);
            }
            StageDefinition.Update(this);
            Callbacks.PostLevelUpdate.Run(this);
        }
        #endregion

        #region 属性
        public void SetProperty(string name, object value)
        {
            propertyDict.SetProperty(name, value);
        }
        public object GetProperty(string name, bool ignoreStageDefinition = false, bool ignoreAreaDefinition = false)
        {
            object result = null;
            if (propertyDict.TryGetProperty(name, out var value))
                result = value;
            else if (!ignoreStageDefinition && StageDefinition.TryGetProperty<object>(name, out var stageProp))
                result = stageProp;
            else if (!ignoreAreaDefinition && AreaDefinition.TryGetProperty<object>(name, out var areaProp))
                result = areaProp;
            return result;
        }
        public T GetProperty<T>(string name, bool ignoreStageDefinition = false, bool ignoreAreaDefinition = false)
        {
            return GetProperty(name, ignoreStageDefinition, ignoreAreaDefinition).ToGeneric<T>();
        }
        #endregion

        #region 坐标相关方法
        public int GetGridIndex(int column, int lane)
        {
            return column + lane * GetMaxColumnCount();
        }
        public float GetGridWidth()
        {
            return gridWidth;
        }
        public float GetGridHeight()
        {
            return gridHeight;
        }
        public float GetGridRightX()
        {
            return GetGridLeftX() + GetMaxColumnCount() * GetGridWidth();
        }
        public float GetGridLeftX()
        {
            return gridLeftX;
        }
        public float GetGridTopZ()
        {
            return GetGridBottomZ() + GetMaxLaneCount() * GetGridHeight();
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
            return Mathf.FloorToInt((GetGridTopZ() - z) / GetGridHeight());
        }
        public int GetColumn(float x)
        {
            //2.4 - 3.2 : 0
            //3.2 - 4.0 : 1
            //4.0 - 4.8 : 2
            //5.6 - 6.2 : 3
            return Mathf.FloorToInt((x - GetGridLeftX()) / GetGridWidth());
        }
        public float GetEntityLaneZ(int row)
        {
            return GetLaneZ(row) + 16;
        }
        public float GetEntityColumnX(int column)
        {
            return GetColumnX(column) + GetGridWidth() * 0.5f;
        }
        public float GetColumnX(int column)
        {
            return GetGridLeftX() + column * GetGridWidth();
        }
        public float GetLaneZ(int lane)
        {
            return GetGridTopZ() - (lane + 1) * GetGridHeight();
        }
        public float GetGroundHeight(Vector3 pos)
        {
            return GetGroundY(pos.x, pos.z);
        }
        public float GetGroundY(float x, float z)
        {
            return 0;
        }
        public LawnGrid GetGrid(int index)
        {
            if (index < 0 || index >= GetMaxColumnCount() * GetMaxLaneCount())
                return null;
            return grids[index];
        }

        public LawnGrid GetGrid(int column, int lane)
        {
            if (column < 0 || column >= GetMaxColumnCount() || lane < 0 || lane >= GetMaxLaneCount())
                return null;
            return GetGrid(lane * GetMaxColumnCount() + column);
        }

        public LawnGrid GetGrid(Vector2Int pos)
        {
            return GetGrid(pos.x, pos.y);
        }
        #endregion 坐标相关方法

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

        public SerializableLevel Serialize()
        {
            return new SerializableLevel()
            {
                seed = Seed,
                isCleared = IsCleared,
                stageDefinitionID = StageDefinition.GetID(),
                areaDefinitionID = AreaDefinition.GetID(),
                isEndless = IsEndless,
                difficulty = Difficulty,
                Option = Option.Serialize(),

                levelRandom = levelRandom.Serialize(),
                entityRandom = entityRandom.Serialize(),
                effectRandom = effectRandom.Serialize(),
                roundRandom = roundRandom.Serialize(),
                spawnRandom = spawnRandom.Serialize(),
                conveyorRandom = conveyorRandom.Serialize(),
                debugRandom = debugRandom.Serialize(),
                miscRandom = miscRandom.Serialize(),

                propertyDict = propertyDict.Serialize(),
                grids = grids.Select(g => g.Serialize()).ToArray(),
                rechargeSpeed = RechargeSpeed,
                rechargeTimeMultiplier = RechargeTimeMultiplier,
                seedPacks = seedPacks.ConvertAll(g => g.Serialize()),
                requireCards = RequireCards,

                currentEntityID = 1,
                entities = entities.ConvertAll(e => e.Serialize()),

                energy = Energy,
                delayedEnergyEntities = delayedEnergyEntities.ToDictionary(d => d.Key.ID, d => d.Value),

                currentWave = CurrentWave,
                currentFlag = CurrentFlag,
                waveState = WaveState,
                levelProgressVisible = LevelProgressVisible,
                spawnedLanes = spawnedLanes,
                spawnedID = spawnedID,
            };
        }
        public static Level Deserialize(SerializableLevel seri, Mod[] mods)
        {
            var game = new Level(mods);
            game.Seed = seri.seed;
            game.IsCleared = seri.isCleared;
            game.StageDefinition = game.GetStageDefinition(seri.stageDefinitionID);
            game.AreaDefinition = game.GetAreaDefinition(seri.areaDefinitionID);
            game.InitAreaProperties();

            game.IsEndless = seri.isEndless;
            game.Difficulty = seri.difficulty;
            game.Option = LevelOption.Deserialize(seri.Option);
            game.entities = seri.entities.ConvertAll(e => Entity.CreateDeserializingEntity(e, game));
            for (int i = 0; i < game.entities.Count; i++)
            {
                game.entities[i].ApplyDeserialize(seri.entities[i]);
            }

            game.levelRandom = RandomGenerator.Deserialize(seri.levelRandom);
            game.entityRandom = RandomGenerator.Deserialize(seri.entityRandom);
            game.effectRandom = RandomGenerator.Deserialize(seri.effectRandom);
            game.roundRandom = RandomGenerator.Deserialize(seri.roundRandom);
            game.spawnRandom = RandomGenerator.Deserialize(seri.spawnRandom);
            game.conveyorRandom = RandomGenerator.Deserialize(seri.conveyorRandom);
            game.debugRandom = RandomGenerator.Deserialize(seri.debugRandom);
            game.miscRandom = RandomGenerator.Deserialize(seri.miscRandom);

            game.propertyDict = PropertyDictionary.Deserialize(seri.propertyDict, game);
            game.grids = seri.grids.Select(g => LawnGrid.Deserialize(g, game)).ToArray();

            game.RechargeSpeed = seri.rechargeSpeed;
            game.RechargeTimeMultiplier = seri.rechargeTimeMultiplier;
            game.seedPacks = seri.seedPacks.ConvertAll(g => SeedPack.Deserialize(g));
            game.RequireCards = seri.requireCards;

            game.currentEntityID = seri.currentEntityID;

            game.Energy = seri.energy;
            game.delayedEnergyEntities = seri.delayedEnergyEntities.ToDictionary(d => game.FindEntityByID(d.Key), d => d.Value);

            game.CurrentWave = seri.currentWave;
            game.CurrentFlag = seri.currentFlag;
            game.WaveState = seri.waveState;
            game.LevelProgressVisible = seri.levelProgressVisible;
            game.spawnedLanes = seri.spawnedLanes;
            game.spawnedID = seri.spawnedID;
            return game;
        }

        #endregion

        #region 私有方法
        public void InitAreaProperties()
        {
            gridWidth = AreaDefinition.GetProperty<float>(AreaProperties.GRID_WIDTH);
            gridHeight = AreaDefinition.GetProperty<float>(AreaProperties.GRID_HEIGHT);
            gridLeftX = AreaDefinition.GetProperty<float>(AreaProperties.GRID_LEFT_X);
            gridBottomZ = AreaDefinition.GetProperty<float>(AreaProperties.GRID_BOTTOM_Z);
            maxLaneCount = AreaDefinition.GetProperty<int>(AreaProperties.MAX_LANE_COUNT);
            maxColumnCount = AreaDefinition.GetProperty<int>(AreaProperties.MAX_COLUMN_COUNT);
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
        public Entity KillerEnemy { get; private set; }
        public bool IsEndless { get; set; }
        /// <summary>
        /// 游戏是否已经通关过一次。
        /// </summary>
        public bool IsRerun { get; set; }
        public int Difficulty { get; set; }
        public int TPS => Option.TPS;
        public LevelOption Option { get; private set; }
        private RandomGenerator levelRandom;

        private RandomGenerator entityRandom;
        private RandomGenerator effectRandom;

        private RandomGenerator roundRandom;
        private RandomGenerator spawnRandom;
        private RandomGenerator conveyorRandom;

        private RandomGenerator debugRandom;
        private RandomGenerator miscRandom;

        private string deathMessage;

        private PropertyDictionary propertyDict = new PropertyDictionary();
        private LawnGrid[] grids;

        private float gridWidth;
        private float gridHeight;
        private float gridLeftX;
        private float gridBottomZ;
        private int maxLaneCount;
        private int maxColumnCount;
        #endregion 保存属性
    }
}