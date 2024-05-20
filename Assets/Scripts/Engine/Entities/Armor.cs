using System;
using System.Collections.Generic;
using System.Linq;

namespace PVZEngine
{
    public class Armor : IBuffTarget
    {
        public Armor(Entity owner)
        {
            Owner = owner;
        }
        public void SetDefinition(ArmorDefinition definition)
        {
            Definition = definition;
        }
        public void Reset()
        {
            MaxHealth = GetProperty<float>(ArmorProperties.MAX_HEALTH);
            Health = MaxHealth;
        }

        #region 属性
        public object GetProperty(string name, bool ignoreDefinition = false, bool ignoreBuffs = false)
        {
            object result = null;
            if (propertyDict.TryGetValue(name, out var prop))
                result = prop;
            else if (!ignoreDefinition)
                result = Definition.GetProperty<object>(name);

            if (!ignoreBuffs)
            {
                foreach (var buff in buffs)
                {
                    foreach (var modi in buff.GetModifiers())
                    {
                        if (modi.PropertyName != name)
                            continue;
                        result = modi.CalculateProperty(buff, result);
                    }
                }
            }
            return result;
        }
        public T GetProperty<T>(string name, bool ignoreDefinition = false, bool ignoreBuffs = false)
        {
            return PropertyDictionary.ToGeneric<T>(GetProperty(name, ignoreDefinition, ignoreBuffs));
        }
        public void SetProperty(string name, object value)
        {
            propertyDict[name] = value;
        }
        #endregion

        #region 原版属性
        public NamespaceID GetShellID(bool ignoreBuffs = false)
        {
            return GetProperty<NamespaceID>(EntityProperties.SHELL, ignoreBuffs: ignoreBuffs);
        }
        public void SetShellID(NamespaceID value)
        {
            SetProperty(EntityProperties.SHELL, value);
        }
        #endregion
        #region 增益
        public void AddBuff(Buff buff)
        {
            if (buff == null)
                return;
            buffs.Add(buff);
            buff.AddToTarget(this);
        }
        public bool RemoveBuff(Buff buff)
        {
            if (buff == null)
                return false;
            if (buffs.Remove(buff))
            {
                buff.RemoveFromTarget();
                return true;
            }
            return false;
        }
        public bool HasBuff(Buff buff)
        {
            return buffs.Contains(buff);
        }
        public Buff[] GetBuffs<T>() where T : BuffDefinition
        {
            return buffs.Where(b => b.Definition is T).ToArray();
        }
        public Buff[] GetAllBuffs()
        {
            return buffs.ToArray();
        }
        #endregion

        public bool Exists()
        {
            return Owner != null && Definition != null && Health > 0;
        }
        #region 属性字段
        public Entity Owner { get; set; }
        public ArmorDefinition Definition { get; private set; }
        public float Health { get; set; }
        public float MaxHealth { get; private set; }
        private List<Buff> buffs = new List<Buff>();
        private Dictionary<string, object> propertyDict = new Dictionary<string, object>();
        #endregion
    }
}