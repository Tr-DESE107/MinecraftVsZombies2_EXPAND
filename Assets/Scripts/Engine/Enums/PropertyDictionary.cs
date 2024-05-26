using System.Collections.Generic;
using System.Linq;
using PVZEngine.Serialization;

namespace PVZEngine
{
    public class PropertyDictionary
    {
        public void SetProperty(string name, object value)
        {
            propertyDict[name] = value;
        }
        public object GetProperty(string name)
        {
            if (TryGetProperty(name, out var prop))
                return prop;
            return null;
        }
        public bool TryGetProperty(string name, out object value)
        {
            return propertyDict.TryGetValue(name, out value);
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
                if (TryToGeneric<T>(prop, out var result))
                {
                    value = ToGeneric<T>(prop);
                    return true;
                }
            }
            value = default;
            return false;
        }
        public static T ToGeneric<T>(object value)
        {
            if (TryToGeneric<T>(value, out var result))
                return result;
            return default;
        }
        public static bool TryToGeneric<T>(object value, out T result)
        {
            if (value is int intValue && typeof(T) == typeof(float))
            {
                var floatValue = (float)intValue;
                if (floatValue is T floatResult)
                {
                    result = floatResult;
                    return true;
                }
            }
            if (value is T tProp)
            {
                result = tProp;
                return true;
            }
            result = default;
            return false;
        }
        public SerializablePropertyDictionary Serialize()
        {
            return new SerializablePropertyDictionary()
            {
                propertyDict = propertyDict.ToDictionary(p => p.Key, p => p.Value)
            };
        }
        public static PropertyDictionary Deserialize(SerializablePropertyDictionary seri, Game level)
        {
            return new PropertyDictionary()
            {
                propertyDict = seri.propertyDict.ToDictionary(p => p.Key, p => p.Value)
            };
        }
        private Dictionary<string, object> propertyDict = new Dictionary<string, object>();
    }
}
