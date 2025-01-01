using System.Collections.Generic;
using System.Linq;
using Tools;

namespace PVZEngine
{
    public class PropertyDictionary
    {
        public bool SetProperty(string name, object value)
        {
            name = string.Intern(name);
            if (value == null)
            {
                if (!propertyDict.TryGetValue(name, out var valueBefore) || valueBefore == null)
                    return false;
            }
            else
            {
                if (propertyDict.TryGetValue(name, out var valueBefore) && value.Equals(valueBefore))
                    return false;
            }
            propertyDict[name] = value;
            return true;
        }
        public object GetProperty(string name)
        {
            if (TryGetProperty(name, out var prop))
                return prop;
            return null;
        }
        public bool TryGetProperty(string name, out object value)
        {
            return propertyDict.TryGetValue(string.Intern(name), out value);
        }
        public T GetProperty<T>(string name)
        {
            if (TryGetProperty<T>(name, out var value))
                return value;
            return default;
        }
        public bool TryGetProperty<T>(string name, out T value)
        {
            if (TryGetProperty(name, out object prop))
            {
                if (prop.TryToGeneric<T>(out var result))
                {
                    value = result;
                    return true;
                }
            }
            value = default;
            return false;
        }
        public bool RemoveProperty(string name)
        {
            return propertyDict.Remove(name);
        }
        public string[] GetPropertyNames()
        {
            return propertyDict.Keys.ToArray();
        }
        public SerializablePropertyDictionary Serialize()
        {
            return new SerializablePropertyDictionary(propertyDict);
        }
        public static PropertyDictionary Deserialize(SerializablePropertyDictionary seri)
        {
            return new PropertyDictionary()
            {
                propertyDict = seri.ToDictionary(p => string.Intern(p.Key), p => p.Value)
            };
        }
        private Dictionary<string, object> propertyDict = new Dictionary<string, object>();
    }
}
