using System;
using System.Collections.Generic;
using System.Linq;
using Tools;
using UnityEngine;

namespace PVZEngine
{
    public class PropertyDictionary
    {
        public bool SetProperty(PropertyKey key, object value)
        {
            if (value is IntPtr ptr)
            {
                value = ptr.ToInt32();
                Debug.LogWarning($"Trying to set a property named \"{key}\" of type IntPtr! Converted it into Int32 {value}.");
            }
            if (value == null)
            {
                if (!propertyDict.TryGetValue(key, out var valueBefore) || valueBefore == null)
                    return false;
            }
            else
            {
                if (propertyDict.TryGetValue(key, out var valueBefore) && value.Equals(valueBefore))
                    return false;
            }
            propertyDict[key] = value;
            return true;
        }
        public object GetProperty(PropertyKey name)
        {
            if (TryGetProperty(name, out var prop))
                return prop;
            return null;
        }
        public bool TryGetProperty(PropertyKey name, out object value)
        {
            return propertyDict.TryGetValue(name, out value);
        }
        public T GetProperty<T>(PropertyKey name)
        {
            if (TryGetProperty<T>(name, out var value))
                return value;
            return default;
        }
        public bool TryGetProperty<T>(PropertyKey name, out T value)
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
        public bool RemoveProperty(PropertyKey name)
        {
            return propertyDict.Remove(name);
        }
        public PropertyKey[] GetPropertyNames()
        {
            return propertyDict.Keys.ToArray();
        }
        public SerializablePropertyDictionary ToSerializable()
        {
            var properties = new Dictionary<string, object>();
            foreach (var pair in propertyDict)
            {
                var key = PropertyMapper.ConvertToFullName(pair.Key);
                if (string.IsNullOrEmpty(key))
                {
                    Debug.LogWarning($"Trying to serialize a property with key {pair.Key}, which is not registered.");
                    continue;
                }
                var value = pair.Value;
                if (value is IntPtr ptr)
                {
                    value = ptr.ToInt32();
                    Debug.LogWarning($"Trying to serialize a property named \"{key}\" of type IntPtr! Converted it into Int32 {value}.");
                }
                properties.Add(key, value);
            }
            return new SerializablePropertyDictionary()
            {
                properties = properties
            };
        }
        public static PropertyDictionary FromSerializable(SerializablePropertyDictionary seri)
        {
            var dict = new PropertyDictionary();
            dict.propertyDict.Clear();
            if (seri.properties != null)
            {
                foreach (var pair in seri.properties)
                {
                    var key = PropertyMapper.ConvertFromName(pair.Key);
                    var value = pair.Value;
                    if (value is IntPtr ptr)
                    {
                        value = ptr.ToInt32();
                        Debug.LogWarning($"Trying to deserialize a property named \"{key}\" of type IntPtr! Converted it into Int32 {value}.");
                    }
                    dict.propertyDict.Add(key, value);
                }
            }
            return dict;
        }
        public int Count => propertyDict.Count;
        private Dictionary<PropertyKey, object> propertyDict = new Dictionary<PropertyKey, object>(32);
    }
    [Serializable]
    public class SerializablePropertyDictionary
    {
        public Dictionary<string, object> properties;
    }
}
