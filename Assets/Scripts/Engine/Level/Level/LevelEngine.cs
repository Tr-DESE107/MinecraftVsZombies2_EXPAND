using System;
using System.Collections.Generic;
using System.Linq;
using PVZEngine.Armors;
using PVZEngine.Buffs;
using PVZEngine.Callbacks;
using PVZEngine.Definitions;
using PVZEngine.Entities;
using PVZEngine.Level.Collisions;
using PVZEngine.Models;
using PVZEngine.Modifiers;
using PVZEngine.SeedPacks;
using UnityEngine;

namespace PVZEngine.Level
{
    public partial class LevelEngine : IBuffTarget, IDisposable, IPropertyModifyTarget
    {
        #region 公有方法
        public LevelEngine(IGameContent contentProvider, IGameLocalization translator, IGameTriggerSystem triggers, ICollisionSystem collisionSystem)
        {
            Content = contentProvider;
            Localization = translator;
            Triggers = triggers;
            buffs.OnPropertyChanged += UpdateBuffedProperty;
            properties = new PropertyBlock(this);
            this.collisionSystem = collisionSystem;
        }


        public void Dispose()
        {
            RemoveTriggers(addedTriggers);
        }

        #region 组件
        public void AddComponent(ILevelComponent component)
        {
            component.PostAttach(this);
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
            Option = option;
            InitRandom(seed);

            ChangeArea(areaId);
            ChangeStage(stageId);

            Energy = this.GetStartEnergy();

            InitGrids(AreaDefinition);
        }
        public void Setup()
        {
            AreaDefinition.Setup(this);
            StageDefinition.Setup(this);
            Triggers.RunCallback(LevelCallbacks.POST_LEVEL_SETUP, new LevelCallbackParams(this));
        }
        public void Start()
        {
            foreach (var component in levelComponents)
            {
                component.OnStart();
            }
            StageDefinition.Start(this);
            Triggers.RunCallback(LevelCallbacks.POST_LEVEL_START, new LevelCallbackParams(this));
        }
        public void SetDifficulty(NamespaceID difficulty)
        {
            Difficulty = difficulty;
        }
        public void ChangeStage(NamespaceID stageId)
        {
            StageID = stageId;
            StageDefinition = Content.GetStageDefinition(stageId);
            properties.ClearFallbackCaches();
        }
        public void ChangeArea(NamespaceID areaId)
        {
            AreaID = areaId;
            AreaDefinition = Content.GetAreaDefinition(areaId);
            properties.ClearFallbackCaches();
        }
        public void Update()
        {
            ClearEntityTrash();

            UpdateSeedPacks();

            foreach (var component in levelComponents)
            {
                component.Update();
            }

            UpdateDelayedEnergyEntities();
            UpdateEntities();
            CollisionUpdate();

            buffs.Update();
            AreaDefinition.Update(this);
            StageDefinition.Update(this);
            Triggers.RunCallback(LevelCallbacks.POST_LEVEL_UPDATE, new LevelCallbackParams(this));
            AddLevelTime();
        }
        #endregion

        #region 属性
        public T GetProperty<T>(PropertyKey<T> name, bool ignoreBuffs = false)
        {
            return properties.GetProperty<T>(name, ignoreBuffs);
        }
        public bool TryGetProperty<T>(PropertyKey<T> name, out T value, bool ignoreBuffs = false)
        {
            return properties.TryGetProperty<T>(name, out value, ignoreBuffs);
        }
        public void SetProperty<T>(PropertyKey<T> name, T value)
        {
            properties.SetProperty(name, value);
        }
        private void UpdateAllBuffedProperties(bool triggersEvaluation)
        {
            properties.UpdateAllModifiedProperties(triggersEvaluation);
        }
        private void UpdateBuffedProperty(IPropertyKey name)
        {
            properties.UpdateModifiedProperty(name);
        }
        bool IPropertyModifyTarget.GetFallbackProperty(IPropertyKey name, out object value)
        {
            if (StageDefinition != null && StageDefinition.TryGetPropertyObject(name, out var stageProp))
            {
                value = stageProp;
                return true;
            }
            if (AreaDefinition != null && AreaDefinition.TryGetPropertyObject(name, out var areaProp))
            {
                value = areaProp;
                return true;
            }
            value = default;
            return false;
        }

        void IPropertyModifyTarget.GetModifierItems(IPropertyKey name, List<ModifierContainerItem> results)
        {
            buffs.GetModifierItems(name, results);
        }
        void IPropertyModifyTarget.UpdateModifiedProperty(IPropertyKey name, object beforeValue, object afterValue, bool triggersEvaluation)
        {
        }
        PropertyModifier[] IPropertyModifyTarget.GetModifiersUsingProperty(IPropertyKey name)
        {
            return null;
        }
        IEnumerable<IPropertyKey> IPropertyModifyTarget.GetModifiedProperties()
        {
            return buffs.GetModifierPropertyNames();
        }
        #endregion

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
        public Buff AddBuff<T>() where T : BuffDefinition
        {
            var buff = CreateBuff<T>(AllocBuffID());
            AddBuff(buff);
            return buff;
        }
        public Buff AddBuff(NamespaceID buffID)
        {
            var buffDef = Content.GetBuffDefinition(buffID);
            return AddBuff(buffDef);
        }
        public Buff AddBuff(BuffDefinition buffDef)
        {
            if (buffDef == null)
                return null;
            var buff = CreateBuff(buffDef, AllocBuffID());
            AddBuff(buff);
            return buff;
        }
        public bool RemoveBuff(Buff buff) => buffs.RemoveBuff(buff);
        public int RemoveBuffs<T>() where T : BuffDefinition => this.buffs.RemoveBuffs<T>();
        public int RemoveBuffs(IEnumerable<Buff> buffs) => this.buffs.RemoveBuffs(buffs);
        public int RemoveBuffs(BuffDefinition buffDef) => this.buffs.RemoveBuffs(buffDef);
        public bool HasBuff<T>() where T : BuffDefinition => buffs.HasBuff<T>();
        public bool HasBuff(Buff buff) => buffs.HasBuff(buff);
        public bool HasBuff(BuffDefinition buffDef) => buffs.HasBuff(buffDef);
        public Buff[] GetBuffs<T>() where T : BuffDefinition => buffs.GetBuffs<T>();
        public void GetBuffs<T>(List<Buff> results) where T : BuffDefinition => buffs.GetBuffsNonAlloc<T>(results);
        public void GetAllBuffs(List<Buff> results) => buffs.GetAllBuffs(results);
        public BuffReference GetBuffReference(Buff buff) => new BuffReferenceLevel(buff.ID);
        private long AllocBuffID()
        {
            long id = currentBuffID;
            currentBuffID++;
            return id;
        }
        public Buff CreateBuff<T>(long buffID) where T : BuffDefinition
        {
            var buffDefinition = Content.GetBuffDefinition<T>();
            return CreateBuff(buffDefinition, buffID);
        }
        public Buff CreateBuff(NamespaceID id, long buffID)
        {
            var buffDefinition = Content.GetBuffDefinition(id);
            return CreateBuff(buffDefinition, buffID);
        }
        public Buff CreateBuff(BuffDefinition buffDef, long buffID)
        {
            if (buffDef == null)
                return null;
            return new Buff(this, buffDef, buffID);
        }
        public Buff NewBuff<T>() where T : BuffDefinition
        {
            return CreateBuff<T>(AllocBuffID());
        }
        public Buff NewBuff(NamespaceID id)
        {
            return CreateBuff(id, AllocBuffID());
        }
        public Buff NewBuff(BuffDefinition buffDef)
        {
            return CreateBuff(buffDef, AllocBuffID());
        }


        #endregion

        #region 序列化
        public SerializableLevel Serialize()
        {
            var level = new SerializableLevel()
            {
                stageDefinitionID = StageDefinition.GetID(),
                areaDefinitionID = AreaDefinition.GetID(),
                difficulty = Difficulty,
                Option = Option.Serialize(),

                properties = properties.ToSerializable(),
                seedPacks = seedPacks.Select(g => g != null ? g.Serialize() : null).ToArray(),
                conveyorSeedPacks = conveyorSeedPacks.Select(s => s != null ? s.Serialize() : null).ToArray(),
                conveyorSlotCount = conveyorSlotCount,
                conveyorSeedSpendRecord = conveyorSeedSpendRecord.ToSerializable(),
                collisionSystem = collisionSystem.ToSerializable(),

                currentBuffID = currentBuffID,
                currentSeedPackID = currentSeedPackID,

                energy = Energy,
                delayedEnergyEntities = delayedEnergyEntities.Select(d => new SerializableDelayedEnergy() { entityId = d.Key.ID, energy = d.Value }).ToArray(),

                currentWave = CurrentWave,
                currentFlag = CurrentFlag,
                waveState = WaveState,
                levelProgressVisible = LevelProgressVisible,

                buffs = buffs.ToSerializable(),

                components = levelComponents.ToDictionary(c => c.GetID().ToString(), c => c.ToSerializable())
            };
            WriteEntitiesToSerializable(level);
            WriteProgressToSerializable(level);
            WriteRandomToSerializable(level);
            WriteGridsToSerializable(level);
            return level;
        }
        public static LevelEngine Deserialize(SerializableLevel seri, IGameContent provider, IGameLocalization translator, IGameTriggerSystem triggers, ICollisionSystem collisionSystem)
        {
            var level = new LevelEngine(provider, translator, triggers, collisionSystem);
            level.ReadProgressFromSerializable(seri);
            level.ReadRandomFromSerializable(seri);

            level.ChangeStage(seri.stageDefinitionID);
            level.ChangeArea(seri.areaDefinitionID);
            level.InitGrids(level.AreaDefinition);
            level.CreateGridsFromSerializable(seri);

            level.Difficulty = seri.difficulty;
            level.Option = LevelOption.Deserialize(seri.Option);
            level.properties = PropertyBlock.FromSerializable(seri.properties, level);

            level.Energy = seri.energy;
            level.currentBuffID = seri.currentBuffID;
            level.currentSeedPackID = seri.currentSeedPackID;

            level.CurrentWave = seri.currentWave;
            level.CurrentFlag = seri.currentFlag;
            level.WaveState = seri.waveState;
            level.LevelProgressVisible = seri.levelProgressVisible;

            level.conveyorSlotCount = seri.conveyorSlotCount;
            level.conveyorSeedSpendRecord = ConveyorSeedSpendRecords.ToDeserialized(seri.conveyorSeedSpendRecord);

            // 加载所有种子包。
            level.seedPacks = seri.seedPacks.Select(g => g != null ? ClassicSeedPack.Deserialize(g, level) : null).ToArray();
            level.conveyorSeedPacks = seri.conveyorSeedPacks.Select(s => s != null ? ConveyorSeedPack.Deserialize(s, level) : null).ToList();
            // 加载所有实体。
            level.CreateEntitiesFromSerializable(seri);
            // 加载所有BUFF。
            level.buffs = BuffList.FromSerializable(seri.buffs, level, level);
            level.buffs.OnPropertyChanged += level.UpdateBuffedProperty;

            // 所有实体、种子包和BUFF都已加载完毕。


            // 加载所有种子包、实体、BUFF的详细信息。
            // 因为有光环这种东西的存在，可能会引用buff，所以需要在buff加载完之后加载。
            foreach (var seed in level.seedPacks)
            {
                if (seed == null)
                    continue;
                var seriSeed = seri.seedPacks.FirstOrDefault(s => s.id == seed.ID);
                if (seriSeed == null)
                    continue;
                seed.ApplyDeserializedProperties(level, seriSeed);
            }
            foreach (var seed in level.conveyorSeedPacks)
            {
                if (seed == null)
                    continue;
                var seriSeed = seri.conveyorSeedPacks.FirstOrDefault(s => s.id == seed.ID);
                if (seriSeed == null)
                    continue;
                seed.ApplyDeserializedProperties(level, seriSeed);
            }
            level.ReadEntitiesFromSerializable(seri);
            level.buffs.LoadAuras(seri.buffs, level);

            // 在实体加载后面
            level.collisionSystem.LoadFromSerializable(level, seri.collisionSystem);
            // 加载所有网格的属性，需要引用实体。
            level.ReadGridsFromSerializable(seri);

            level.delayedEnergyEntities = seri.delayedEnergyEntities.ToDictionary(d => level.FindEntityByID(d.entityId), d => d.energy);
            level.UpdateAllBuffedProperties(false);

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

        #endregion

        #region 私有方法
        IModelInterface IBuffTarget.GetInsertedModel(NamespaceID key) => null;
        Entity IBuffTarget.GetEntity() => null;
        Armor IBuffTarget.GetArmor() => null;
        void IBuffTarget.GetBuffs(List<Buff> results) => buffs.GetAllBuffs(results);
        Buff IBuffTarget.GetBuff(long id) => buffs.GetBuff(id);
        Buff IBuffTarget.NewBuff(NamespaceID id) => CreateBuff(id, AllocBuffID());
        bool IBuffTarget.Exists() => true;
        #endregion

        #region 属性字段
        public IGameContent Content { get; private set; }
        public IGameLocalization Localization { get; private set; }
        public NamespaceID StageID { get; private set; }
        public StageDefinition StageDefinition { get; private set; }
        public NamespaceID AreaID { get; private set; }
        public AreaDefinition AreaDefinition { get; private set; }
        public NamespaceID Difficulty { get; set; }
        public bool IsRerun { get; set; }
        public int TPS => Option.TPS;
        public LevelOption Option { get; private set; }


        private PropertyBlock properties;
        private BuffList buffs = new BuffList();

        private long currentBuffID = 1;

        private List<ILevelComponent> levelComponents = new List<ILevelComponent>();
        #endregion 保存属性
    }
}