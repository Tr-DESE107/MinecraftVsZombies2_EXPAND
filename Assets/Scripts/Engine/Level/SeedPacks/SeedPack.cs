using System.Collections.Generic;
using PVZEngine.Buffs;
using PVZEngine.Definitions;
using PVZEngine.Entities;
using PVZEngine.Level;
using Tools;
using UnityEngine;

namespace PVZEngine.SeedPacks
{
    public abstract class SeedPack : IBuffTarget
    {
        public SeedPack(LevelEngine level, SeedDefinition definition, long id)
        {
            ID = id;
            Level = level;
            Definition = definition;
            buffs.OnPropertyChanged += UpdateBuffedProperty;
        }
        public abstract int GetIndex();
        public NamespaceID GetDefinitionID()
        {
            return Definition?.GetID();
        }
        #region 属性
        public object GetProperty(string name, bool ignoreDefinition = false, bool ignoreBuffs = false)
        {
            if (!ignoreBuffs)
            {
                if (buffedProperties.TryGetProperty(name, out var v))
                    return v;
            }
            object result = null;
            if (propertyDict.TryGetProperty(name, out var prop))
                result = prop;
            else if (!ignoreDefinition)
                result = Definition.GetProperty<object>(name);
            return result;
        }
        public T GetProperty<T>(string name, bool ignoreDefinition = false, bool ignoreBuffs = false)
        {
            return GetProperty(name, ignoreDefinition, ignoreBuffs).ToGeneric<T>();
        }
        public void SetProperty(string name, object value)
        {
            propertyDict.SetProperty(name, value);
            UpdateBuffedProperty(name);
        }
        private void UpdateAllBuffedProperties()
        {
            var propertyNames = buffs.GetModifierPropertyNames();
            foreach (var name in propertyNames)
            {
                UpdateBuffedProperty(name);
            }
        }
        private void UpdateBuffedProperty(string name)
        {
            var baseValue = GetProperty(name, ignoreBuffs: true);
            var value = buffs.CalculateProperty(name, baseValue);
            buffedProperties.SetProperty(name, value);
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
        public abstract BuffReference GetBuffReference(Buff buff);
        private long AllocBuffID()
        {
            return currentBuffID++;
        }
        #endregion

        #region 消耗
        public int GetCost()
        {
            var cost = GetProperty<float>(EngineSeedProps.COST);
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
            if (!this.IsCharged())
            {
                var recharge = this.GetRecharge();
                recharge += rechargeSpeed * this.GetRechargeSpeed();
                recharge = Mathf.Min(this.GetMaxRecharge(), recharge);
                this.SetRecharge(recharge);
            }
            foreach (var buff in buffs.GetAllBuffs())
            {
                buff.Update();
            }
        }
        #endregion

        #region 序列化
        protected void ApplySerializableProperties(SerializableSeedPack seri)
        {
            seri.id = ID;
            seri.seedID = Definition.GetID();
            seri.propertyDict = propertyDict.Serialize();
            seri.buffs = buffs.ToSerializable();
            seri.currentBuffID = currentBuffID;
        }
        protected void ApplyDeserializedProperties(LevelEngine level, SerializableSeedPack seri)
        {
            propertyDict = PropertyDictionary.Deserialize(seri.propertyDict);
            currentBuffID = seri.currentBuffID;
            buffs = BuffList.FromSerializable(seri.buffs, level, this);
            buffs.OnPropertyChanged += UpdateBuffedProperty;
            UpdateAllBuffedProperties();
        }
        #endregion

        IEnumerable<Buff> IBuffTarget.GetBuffs() => buffs.GetAllBuffs();
        Entity IBuffTarget.GetEntity() => null;
        bool IBuffTarget.Exists() => true;

        #region 属性字段
        public long ID { get; }
        public LevelEngine Level { get; private set; }
        public SeedDefinition Definition { get; private set; }
        protected long currentBuffID = 1;
        protected PropertyDictionary buffedProperties = new PropertyDictionary();
        protected PropertyDictionary propertyDict = new PropertyDictionary();
        protected BuffList buffs = new BuffList();
        #endregion
    }
}
