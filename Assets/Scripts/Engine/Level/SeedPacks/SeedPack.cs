using System;
using System.Collections.Generic;
using System.Linq;
using PVZEngine.Armors;
using PVZEngine.Auras;
using PVZEngine.Buffs;
using PVZEngine.Definitions;
using PVZEngine.Entities;
using PVZEngine.Level;
using PVZEngine.Models;
using PVZEngine.Modifiers;
using UnityEngine;

namespace PVZEngine.SeedPacks
{
    public abstract class SeedPack : IBuffTarget, IPropertyModifyTarget, IAuraSource
    {
        public SeedPack(LevelEngine level, SeedDefinition definition, long id)
        {
            ID = id;
            Level = level;
            Definition = definition;
            buffs.OnPropertyChanged += UpdateBuffedProperty;
            buffs.OnModelInsertionAdded += OnModelInsertionAddedCallback;
            buffs.OnModelInsertionRemoved += OnModelInsertionRemovedCallback;
            properties = new PropertyBlock(this);

            var auraCount = definition.GetAuraCount();
            for (int i = 0; i < auraCount; i++)
            {
                var auraDef = definition.GetAuraAt(i);
                auras.Add(level, new AuraEffect(auraDef, i, this));
            }
        }
        public NamespaceID GetDefinitionID()
        {
            return Definition?.GetID();
        }
        public void ChangeDefinition(SeedDefinition definition)
        {
            Definition = definition;
            UpdateAllBuffedProperties();
            OnDefinitionChanged?.Invoke(definition);
        }

        public void PostAdd(LevelEngine level)
        {
            auras.PostAdd();
        }
        public void PostRemove(LevelEngine level)
        {
            auras.PostRemove();
        }

        #region 属性
        public T GetProperty<T>(PropertyKey<T> name, bool ignoreBuffs = false)
        {
            return properties.GetProperty<T>(name, ignoreBuffs);
        }
        public void SetProperty<T>(PropertyKey<T> name, T value)
        {
            properties.SetProperty(name, value);
        }
        private void UpdateAllBuffedProperties()
        {
            properties.UpdateAllModifiedProperties();
        }
        private void UpdateBuffedProperty(IPropertyKey name)
        {
            properties.UpdateModifiedProperty(name);
        }
        bool IPropertyModifyTarget.GetFallbackProperty<T>(PropertyKey<T> name, out T value)
        {
            if (Definition != null && Definition.TryGetProperty(name, out var prop))
            {
                value = prop;
                return true;
            }
            value = default;
            return false;
        }

        void IPropertyModifyTarget.GetModifierItems(IPropertyKey name, List<ModifierContainerItem> results)
        {
            buffs.GetModifierItems(name, results);
        }
        void IPropertyModifyTarget.UpdateModifiedProperty<T>(PropertyKey<T> name, T beforeValue, T afterValue)
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

        #region 增益
        public Buff CreateBuff<T>() where T : BuffDefinition
        {
            return Level.CreateBuff<T>(AllocBuffID());
        }
        public Buff CreateBuff(BuffDefinition buffDef)
        {
            return Level.CreateBuff(buffDef, AllocBuffID());
        }
        public Buff CreateBuff(NamespaceID id)
        {
            return Level.CreateBuff(id, AllocBuffID());
        }
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
            var buff = CreateBuff<T>();
            AddBuff(buff);
            return buff;
        }
        public bool RemoveBuff(Buff buff) => buffs.RemoveBuff(buff);
        public int RemoveBuffs(IEnumerable<Buff> buffs) => this.buffs.RemoveBuffs(buffs);
        public bool HasBuff<T>() where T : BuffDefinition => buffs.HasBuff<T>();
        public bool HasBuff(Buff buff) => buffs.HasBuff(buff);
        public Buff[] GetBuffs<T>() where T : BuffDefinition => buffs.GetBuffs<T>();
        public void GetAllBuffs(List<Buff> results) => buffs.GetAllBuffs(results);
        public abstract BuffReference GetBuffReference(Buff buff);
        private long AllocBuffID()
        {
            return currentBuffID++;
        }
        #endregion

        #region 消耗
        public int GetCost()
        {
            float cost = GetProperty<float>(EngineSeedProps.COST);
            cost = Mathf.Max(cost, 0);
            return Mathf.FloorToInt(cost);
        }
        #endregion

        #region 充能
        public RechargeDefinition GetRechargeDefinition()
        {
            var rechargeID = this.GetRechargeID();
            if (rechargeID == null)
                return null;
            return Level.Content.GetRechargeDefinition(rechargeID);
        }
        public int GetStartMaxRecharge()
        {
            var rechargeDef = GetRechargeDefinition();
            if (rechargeDef == null)
                return 0;
            return rechargeDef.GetStartMaxRecharge();
        }
        public int GetUsedMaxRecharge()
        {
            var rechargeDef = GetRechargeDefinition();
            if (rechargeDef == null)
                return 0;
            return rechargeDef.GetMaxRecharge();
        }
        public void Update(float rechargeSpeed)
        {
            auras.Update();
            OnUpdate(rechargeSpeed);
            buffs.Update();
            Definition.Update(this, rechargeSpeed);
        }
        protected virtual void OnUpdate(float rechargeSpeed)
        {

        }
        #endregion

        #region 模型
        public void SetModelInterface(IModelInterface model)
        {
            modelInterface = model;
        }
        public IModelInterface CreateChildModel(string anchorName, NamespaceID key, NamespaceID modelID)
        {
            return modelInterface.CreateChildModel(anchorName, key, modelID);
        }
        public bool RemoveChildModel(NamespaceID key)
        {
            return modelInterface.RemoveChildModel(key);
        }
        public IModelInterface GetChildModel(NamespaceID key)
        {
            return modelInterface.GetChildModel(key);
        }
        private void OnModelInsertionAddedCallback(string anchorName, NamespaceID key, NamespaceID modelID)
        {
            modelInterface.CreateChildModel(anchorName, key, modelID);
        }
        private void OnModelInsertionRemovedCallback(NamespaceID key)
        {
            modelInterface.RemoveChildModel(key);
        }
        #endregion

        #region 序列化
        protected void ApplySerializableProperties(SerializableSeedPack seri)
        {
            seri.id = ID;
            seri.seedID = Definition.GetID();
            seri.properties = properties.ToSerializable();
            seri.buffs = buffs.ToSerializable();
            seri.currentBuffID = currentBuffID;
            seri.auras = auras.GetAll().Select(a => a.ToSerializable()).ToArray();
        }
        protected void ApplyDeserializedProperties(LevelEngine level, SerializableSeedPack seri)
        {
            properties = PropertyBlock.FromSerializable(seri.properties, this);
            currentBuffID = seri.currentBuffID;
            buffs = BuffList.FromSerializable(seri.buffs, level, this);
            buffs.OnPropertyChanged += UpdateBuffedProperty;
            buffs.OnModelInsertionAdded += OnModelInsertionAddedCallback;
            buffs.OnModelInsertionRemoved += OnModelInsertionRemovedCallback;
            auras.LoadFromSerializable(level, seri.auras);
            UpdateAllBuffedProperties();
        }
        #endregion

        IModelInterface IBuffTarget.GetInsertedModel(NamespaceID key) => GetChildModel(key);
        void IBuffTarget.GetBuffs(List<Buff> results) => buffs.GetAllBuffs(results);
        Buff IBuffTarget.GetBuff(long id) => buffs.GetBuff(id);
        Entity IBuffTarget.GetEntity() => null;
        Armor IBuffTarget.GetArmor() => null;
        Entity IAuraSource.GetEntity() => null;
        LevelEngine IAuraSource.GetLevel() => Level;
        bool IBuffTarget.Exists() => true;
        public event Action<SeedDefinition> OnDefinitionChanged;

        #region 属性字段
        public long ID { get; }
        public LevelEngine Level { get; private set; }
        public SeedDefinition Definition { get; private set; }
        private IModelInterface modelInterface;
        protected long currentBuffID = 1;
        private PropertyBlock properties;
        protected BuffList buffs = new BuffList();
        protected AuraEffectList auras = new AuraEffectList();
        #endregion
    }
}
