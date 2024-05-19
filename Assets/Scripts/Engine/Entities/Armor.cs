using System.Collections.Generic;

namespace PVZEngine
{
    public class Armor
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

        public bool Exists()
        {
            return Owner != null && Definition != null && Health > 0;
        }
        #region 属性字段
        public Entity Owner { get; set; }
        public ArmorDefinition Definition { get; private set; }
        public float Health { get; set; }
        public float MaxHealth { get; private set; }
        private Dictionary<string, object> propertyDict = new Dictionary<string, object>();
        #endregion
    }
}