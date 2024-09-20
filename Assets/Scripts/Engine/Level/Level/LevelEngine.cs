using System;
using System.Collections.Generic;
using System.Linq;
using log4net.Core;
using PVZEngine.Definitions;
using PVZEngine.Serialization;
using Tools;
using UnityEngine;

namespace PVZEngine.Level
{
    public partial class LevelEngine : IBuffTarget
    {
        #region 公有方法
        public LevelEngine(IContentProvider contentProvider, ITranslator translator)
        {
            ContentProvider = contentProvider;
            Translator = translator;
        }

        #region 组件
        public void AddComponent(ILevelComponent component)
        {
            levelComponents.Add(component);
        }
        public ILevelComponent[] GetComponents()
        {
            return levelComponents.ToArray();
        }
        public T GetComponent<T>() where T : ILevelComponent
        {
            foreach (var comp in levelComponents)
            {
                if (comp is T tComp)
                    return tComp;
            }
            return default;
        }
        #endregion

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


            AreaDefinition = ContentProvider.GetAreaDefinition(areaId);
            ChangeStage(stageId);

            InitAreaProperties();

            // Initalize current stage info.
            var maxLane = GetMaxLaneCount();
            int maxColumn = GetMaxColumnCount();

            grids = new LawnGrid[maxColumn * maxLane];

            var gridDefinitions = AreaDefinition.GetGridDefintionsID().Select(i => ContentProvider.GetGridDefinition(i)).ToArray();
            for (int i = 0; i < gridDefinitions.Length; i++)
            {
                var definition = gridDefinitions[i];
                int lane = Mathf.FloorToInt(i / maxColumn);
                int column = i % maxColumn;
                grids[i] = new LawnGrid(this, definition, lane, column);
            }
        }
        public void SetupArea()
        {
            AreaDefinition.Setup(this);
        }
        public void Start()
        {
            StageDefinition.Start(this);
            LevelCallbacks.PostLevelStart.Run(this);
        }
        public void SetDifficulty(NamespaceID difficulty)
        {
            Difficulty = difficulty;
        }
        public void ChangeStage(NamespaceID stageId)
        {
            StageID = stageId;
            StageDefinition = ContentProvider.GetStageDefinition(stageId);
        }
        public void Update()
        {
            UpdateSeedPacks();
            collisionCachedBounds.Clear();
            var entities = GetEntities();
            foreach (var entity in entities)
            {
                entity.Update();
                CollisionUpdate(entity, entity.CollisionMask, entities);
            }
            foreach (var buff in buffs.GetAllBuffs())
            {
                buff.Update();
            }
            foreach (var component in levelComponents)
            {
                component.Update();
            }
            StageDefinition.Update(this);
            LevelCallbacks.PostLevelUpdate.Run(this);
        }
        #endregion

        #region 属性
        public void SetProperty(string name, object value)
        {
            propertyDict.SetProperty(name, value);
        }
        public object GetProperty(string name, bool ignoreStageDefinition = false, bool ignoreAreaDefinition = false, bool ignoreBuffs = false)
        {
            object result = null;
            if (propertyDict.TryGetProperty(name, out var value))
                result = value;
            else if (!ignoreStageDefinition && StageDefinition.TryGetProperty<object>(name, out var stageProp))
                result = stageProp;
            else if (!ignoreAreaDefinition && AreaDefinition.TryGetProperty<object>(name, out var areaProp))
                result = areaProp;

            if (!ignoreBuffs)
            {
                result = buffs.CalculateProperty(name, result);
            }
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
            return Mathf.FloorToInt((GetGridTopZ() - z) / GetGridHeight());
        }
        public int GetColumn(float x)
        {
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
        public float GetGroundY(Vector3 pos)
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

        #region 增益
        public bool AddBuff(Buff buff)
        {
            if (buffs.AddBuff(buff))
            {
                buff.AddToTarget(this);
                return true;
            }
            return false;
        }
        public void AddBuff<T>() where T : BuffDefinition
        {
            AddBuff(CreateBuff<T>());
        }
        public bool RemoveBuff(Buff buff) => buffs.RemoveBuff(buff);
        public int RemoveBuffs(IEnumerable<Buff> buffs) => this.buffs.RemoveBuffs(buffs);
        public bool HasBuff<T>() where T : BuffDefinition => buffs.HasBuff<T>();
        public bool HasBuff(Buff buff) => buffs.HasBuff(buff);
        public Buff[] GetBuffs<T>() where T : BuffDefinition => buffs.GetBuffs<T>();
        public Buff[] GetAllBuffs() => buffs.GetAllBuffs();
        #endregion

        public Buff CreateBuff<T>() where T : BuffDefinition
        {
            var buffDefinition = ContentProvider.GetBuffDefinition<T>();
            if (buffDefinition == null)
                return null;
            return new Buff(buffDefinition);
        }
        public Buff CreateBuff(NamespaceID id)
        {
            var buffDefinition = ContentProvider.GetBuffDefinition(id);
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
                seedPacks = seedPacks.Select(g => g != null ? g.Serialize() : null).ToArray(),

                currentEntityID = currentEntityID,
                entities = entities.ConvertAll(e => e.Serialize()),

                energy = Energy,
                delayedEnergyEntities = delayedEnergyEntities.Select(d => new SerializableDelayedEnergy() { entityId = d.Key.ID, energy = d.Value }).ToArray(),

                currentWave = CurrentWave,
                currentFlag = CurrentFlag,
                waveState = WaveState,
                levelProgressVisible = LevelProgressVisible,
                spawnedLanes = spawnedLanes,
                spawnedID = spawnedID,

                buffs = buffs.ToSerializable(),

                components = levelComponents.ToDictionary(c => c.GetID().ToString(), c => c.ToSerializable())
            };
        }
        public static LevelEngine Deserialize(SerializableLevel seri, IContentProvider provider, ITranslator translator)
        {
            var level = new LevelEngine(provider, translator);
            level.Seed = seri.seed;
            level.levelRandom = RandomGenerator.Deserialize(seri.levelRandom);
            level.entityRandom = RandomGenerator.Deserialize(seri.entityRandom);
            level.effectRandom = RandomGenerator.Deserialize(seri.effectRandom);
            level.roundRandom = RandomGenerator.Deserialize(seri.roundRandom);
            level.spawnRandom = RandomGenerator.Deserialize(seri.spawnRandom);
            level.conveyorRandom = RandomGenerator.Deserialize(seri.conveyorRandom);
            level.debugRandom = RandomGenerator.Deserialize(seri.debugRandom);
            level.miscRandom = RandomGenerator.Deserialize(seri.miscRandom);

            level.IsCleared = seri.isCleared;
            level.StageDefinition = provider.GetStageDefinition(seri.stageDefinitionID);
            level.AreaDefinition = provider.GetAreaDefinition(seri.areaDefinitionID);
            level.InitAreaProperties();

            level.IsEndless = seri.isEndless;
            level.Difficulty = seri.difficulty;
            level.Option = LevelOption.Deserialize(seri.Option);
            level.grids = seri.grids.Select(g => LawnGrid.Deserialize(g, level)).ToArray();
            level.propertyDict = PropertyDictionary.Deserialize(seri.propertyDict);
            level.buffs = BuffList.FromSerializable(seri.buffs, level);

            level.RechargeSpeed = seri.rechargeSpeed;
            level.RechargeTimeMultiplier = seri.rechargeTimeMultiplier;
            level.seedPacks = seri.seedPacks.Select(g => g != null ? SeedPack.Deserialize(g, level) : null).ToArray();

            level.currentEntityID = seri.currentEntityID;


            level.CurrentWave = seri.currentWave;
            level.CurrentFlag = seri.currentFlag;
            level.WaveState = seri.waveState;
            level.LevelProgressVisible = seri.levelProgressVisible;
            level.spawnedLanes = seri.spawnedLanes;
            level.spawnedID = seri.spawnedID;

            // 在网格加载后面
            level.entities = seri.entities.ConvertAll(e => Entity.CreateDeserializingEntity(e, level));
            for (int i = 0; i < level.entities.Count; i++)
            {
                level.entities[i].ApplyDeserialize(seri.entities[i]);
            }
            // 在实体加载后面
            level.Energy = seri.energy;
            level.delayedEnergyEntities = seri.delayedEnergyEntities.ToDictionary(d => level.FindEntityByID(d.entityId), d => d.energy);

            return level;
        }
        public void DeserializeComponents(SerializableLevel seri)
        {
            foreach (var seriComp in seri.components)
            {
                var comp = levelComponents.FirstOrDefault(c => c.GetID().ToString() == seriComp.Key);
                if (comp == null)
                    continue;
                comp.LoadSerializable(seriComp.Value);
            }
        }

        #endregion

        #region 私有方法

        #region 接口实现
        ISerializeBuffTarget IBuffTarget.SerializeBuffTarget()
        {
            return new SerializableBuffTargetLevel(this);
        }
        #endregion
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
        public IContentProvider ContentProvider { get; private set; }
        public ITranslator Translator { get; private set; }
        public int Seed { get; private set; }
        public bool IsCleared { get; private set; }
        public NamespaceID StageID { get; private set; }
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
        public NamespaceID Difficulty { get; set; }
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
        private BuffList buffs = new BuffList();

        private float gridWidth;
        private float gridHeight;
        private float gridLeftX;
        private float gridBottomZ;
        private int maxLaneCount;
        private int maxColumnCount;

        private List<ILevelComponent> levelComponents = new List<ILevelComponent>();
        #endregion 保存属性
    }
}