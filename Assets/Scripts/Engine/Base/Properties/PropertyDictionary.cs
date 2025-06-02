﻿using System;
using System.Collections.Generic;
using System.Linq;
using Tools;
using UnityEngine;

namespace PVZEngine
{
    public class PropertyDictionary
    {
        public bool SetPropertyObject(IPropertyKey key, object value)
        {
            if (key.Key == 0)
            {
                Debug.LogWarning("Trying to set a property with an invalid key!");
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
        public bool SetProperty<T>(PropertyKey<T> key, T value)
        {
            return SetPropertyObject(key, value);
        }
        public object GetPropertyObject(IPropertyKey name)
        {
            if (TryGetPropertyObject(name, out var prop))
                return prop;
            return null;
        }
        public bool TryGetPropertyObject(IPropertyKey name, out object value)
        {
            return propertyDict.TryGetValue(name, out value);
        }
        public T GetProperty<T>(PropertyKey<T> name)
        {
            if (TryGetProperty<T>(name, out var value))
                return value;
            return default;
        }
        public bool TryGetProperty<T>(PropertyKey<T> name, out T value)
        {
            if (TryGetPropertyObject(name, out object prop))
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
        public bool RemovePropertyObject(IPropertyKey name)
        {
            return propertyDict.Remove(name);
        }
        public bool RemoveProperty<T>(PropertyKey<T> name)
        {
            return RemovePropertyObject(name);
        }
        public IPropertyKey[] GetPropertyNames()
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
                properties.Add(key, pair.Value);
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
                    dict.propertyDict.Add(key, pair.Value);
                }
            }
            return dict;
        }
        public int Count => propertyDict.Count;
        private Dictionary<IPropertyKey, object> propertyDict = new Dictionary<IPropertyKey, object>(32);
    }
    [Serializable]
    public class SerializablePropertyDictionary
    {
        public Dictionary<string, object> properties;
    }
}
