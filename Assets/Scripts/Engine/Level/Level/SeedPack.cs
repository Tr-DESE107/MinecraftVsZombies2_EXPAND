using System.Collections.Generic;
using PVZEngine.Definitions;
using PVZEngine.Serialization;
using Tools;
using UnityEngine;

namespace PVZEngine.Level
{
    public class SeedPack : IBuffTarget
    {
        public SeedPack(LevelEngine level, SeedDefinition definition)
        {
            Level = level;
            Definition = definition;
        }
        public int GetIndex()
        {
            return Level.GetSeedPackIndex(this);
        }
        #region 属性
        public object GetProperty(string name, bool ignoreDefinition = false, bool ignoreBuffs = false)
        {
            object result = null;
            if (propertyDict.TryGetProperty(name, out var prop))
                result = prop;
            else if (!ignoreDefinition)
                result = Definition.GetProperty<object>(name);

            if (!ignoreBuffs)
            {
                result = buffs.CalculateProperty(name, result);
            }
            return result;
        }
        public T GetProperty<T>(string name, bool ignoreDefinition = false, bool ignoreBuffs = false)
        {
            return GetProperty(name, ignoreDefinition, ignoreBuffs).ToGeneric<T>();
        }
        public void SetProperty(string name, object value)
        {
            propertyDict.SetProperty(name, value);
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
            AddBuff(Level.CreateBuff<T>());
        }
        public bool RemoveBuff(Buff buff) => buffs.RemoveBuff(buff);
        public int RemoveBuffs(IEnumerable<Buff> buffs) => this.buffs.RemoveBuffs(buffs);
        public bool HasBuff<T>() where T : BuffDefinition => buffs.HasBuff<T>();
        public bool HasBuff(Buff buff) => buffs.HasBuff(buff);
        public Buff[] GetBuffs<T>() where T : BuffDefinition => buffs.GetBuffs<T>();
        public Buff[] GetAllBuffs() => buffs.GetAllBuffs();
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
            return Level.ContentProvider.GetRechargeDefinition(rechargeID);
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
                recharge += rechargeSpeed;
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
                seedID = Definition.GetID(),
                propertyDict = propertyDict.Serialize(),
                buffs = buffs.ToSerializable()
            };
        }
        public static SeedPack Deserialize(SerializableSeedPack seri, LevelEngine level)
        {
            var definition = level.ContentProvider.GetSeedDefinition(seri.seedID);
            var seedPack = new SeedPack(level, definition)
            {
                propertyDict = PropertyDictionary.Deserialize(seri.propertyDict)
            };
            seedPack.buffs = BuffList.FromSerializable(seri.buffs, level.ContentProvider, seedPack);
            return seedPack;
        }
        #endregion

        Entity IBuffTarget.GetEntity() => null;

        #region 属性字段
        public LevelEngine Level { get; private set; }
        public SeedDefinition Definition { get; private set; }
        private PropertyDictionary propertyDict = new PropertyDictionary();
        private BuffList buffs = new BuffList();
        #endregion
    }
}
