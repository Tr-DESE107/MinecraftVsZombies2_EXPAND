using System.Collections.Generic;
using PVZEngine.Buffs;
using PVZEngine.Definitions;
using PVZEngine.Entities;
using PVZEngine.Level;
using Tools;
using UnityEngine;

namespace PVZEngine.SeedPacks
{
    public class SeedPack : IBuffTarget
    {
        public SeedPack(LevelEngine level, SeedDefinition definition, int id)
        {
            ID = id;
            Level = level;
            Definition = definition;
            buffs.OnPropertyChanged += UpdateBuffedProperty;
        }
        public int GetIndex()
        {
            return Level.GetSeedPackIndex(this);
        }
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
        public BuffReference GetBuffReference(Buff buff) => new BuffReferenceSeedPack(ID);
        private long AllocBuffID()
        {
            return currentBuffID++;
        }
        #endregion

        #region 消耗
        public int GetCost()
        {
            var cost = GetProperty<int>(EngineSeedProps.COST);
            cost = Mathf.Max(cost, 0);
            return cost;
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
        public SerializableSeedPack Serialize()
        {
            return new SerializableSeedPack()
            {
                id = ID,
                seedID = Definition.GetID(),
                propertyDict = propertyDict.Serialize(),
                buffs = buffs.ToSerializable(),
                currentBuffID = currentBuffID,
            };
        }
        public static SeedPack Deserialize(SerializableSeedPack seri, LevelEngine level)
        {
            var definition = level.Content.GetSeedDefinition(seri.seedID);
            var seedPack = new SeedPack(level, definition, seri.id)
            {
                propertyDict = PropertyDictionary.Deserialize(seri.propertyDict),
                currentBuffID = seri.currentBuffID
            };
            seedPack.buffs = BuffList.FromSerializable(seri.buffs, level, seedPack); ;
            seedPack.buffs.OnPropertyChanged += seedPack.UpdateBuffedProperty;
            seedPack.UpdateAllBuffedProperties();
            return seedPack;
        }
        #endregion

        IEnumerable<Buff> IBuffTarget.GetBuffs() => buffs.GetAllBuffs();
        Entity IBuffTarget.GetEntity() => null;
        bool IBuffTarget.Exists() => true;

        #region 属性字段
        public int ID { get; }
        public LevelEngine Level { get; private set; }
        public SeedDefinition Definition { get; private set; }
        private long currentBuffID = 1;
        private PropertyDictionary buffedProperties = new PropertyDictionary();
        private PropertyDictionary propertyDict = new PropertyDictionary();
        private BuffList buffs = new BuffList();
        #endregion
    }
}
