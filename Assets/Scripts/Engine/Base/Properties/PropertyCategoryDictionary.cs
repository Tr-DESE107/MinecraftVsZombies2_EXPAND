using System;
using System.Collections.Generic;
using System.Linq;
using Tools;

namespace PVZEngine
{
    public class PropertyCategoryDictionary
    {
        private bool TryGetCategory(string category, out PropertyDictionary dict)
        {
            category = string.Intern(category);
            return dictionaries.TryGetValue(category, out dict);
        }
        public bool SetProperty(string category, string name, object value)
        {
            if (!TryGetCategory(category, out var dict))
            {
                dict = new PropertyDictionary();
                dictionaries.Add(category, dict);
            }
            return dict.SetProperty(name, value);
        }
        public object GetProperty(string category, string name)
        {
            if (!TryGetCategory(category, out var dict))
                return null;
            if (dict.TryGetProperty(name, out var prop))
                return prop;
            return null;
        }
        public bool TryGetProperty(string category, string name, out object value)
        {
            if (TryGetCategory(category, out var dict))
            {
                return dict.TryGetProperty(string.Intern(name), out value);
            }
            value = null;
            return false;
        }
        public T GetProperty<T>(string category, string name)
        {
            if (!TryGetCategory(category, out var dict))
                return default;
            if (dict.TryGetProperty<T>(name, out var value))
                return value;
            return default;
        }
        public bool TryGetProperty<T>(string category, string name, out T value)
        {
            if (TryGetCategory(category, out var dict))
            {
                if (dict.TryGetProperty(name, out object prop))
                {
                    if (prop.TryToGeneric<T>(out var result))
                    {
                        value = result;
                        return true;
                    }
                }
            }
            value = default;
            return false;
        }
        public bool RemoveProperty(string category, string name)
        {
            if (!TryGetCategory(category, out var dict))
                return false;
            if (dict.RemoveProperty(name))
            {
                if (dict.Count <= 0)
                {
                    dictionaries.Remove(category);
                }
                return true;
            }
            return false;
        }
        public string[] GetPropertyNames(string category)
        {
            if (TryGetCategory(category, out var dict))
            {
                return dict.GetPropertyNames();
            }
            return Array.Empty<string>();
        }
        public string[] GetCategories()
        {
            return dictionaries.Keys.ToArray();
        }
        public SerializablePropertyCategoryDictionary ToSerializable()
        {
            return new SerializablePropertyCategoryDictionary()
            {
                dictionaries = dictionaries.ToDictionary(p => p.Key, p => p.Value.ToSerializable())
            };
        }
        public static PropertyCategoryDictionary FromSerializable(SerializablePropertyCategoryDictionary seri)
        {
            var dict = new PropertyCategoryDictionary();
            dict.dictionaries.Clear();
            foreach (var pair in seri.dictionaries)
            {
                var category = string.Intern(pair.Key);
                var value = PropertyDictionary.FromSerializable(pair.Value);
                dict.dictionaries.Add(category, value);
            }
            return dict;
        }
        private Dictionary<string, PropertyDictionary> dictionaries = new Dictionary<string, PropertyDictionary>();
    }
    [Serializable]
    public class SerializablePropertyCategoryDictionary
    {
        public Dictionary<string, SerializablePropertyDictionary> dictionaries;
    }
}
