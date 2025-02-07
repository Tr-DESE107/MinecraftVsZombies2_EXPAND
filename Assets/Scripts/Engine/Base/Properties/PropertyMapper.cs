using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace PVZEngine
{
    public static class PropertyMapper
    {
        public static void InitPropertyMaps(string namespaceName, Type[] types)
        {
            foreach (var type in types)
            {
                var regionAttribute = type.GetCustomAttribute<PropertyRegistryRegionAttribute>();
                var definitionAttribute = type.GetCustomAttribute<DefinitionAttribute>();
                foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static))
                {
                    if (!field.IsStatic)
                        continue;
                    var fieldAttribute = field.GetCustomAttribute<PropertyRegistryAttribute>();
                    string regionName;
                    if (fieldAttribute != null)
                    {
                        regionName = fieldAttribute.RegionName;
                    }
                    else if (regionAttribute != null)
                    {
                        regionName = regionAttribute.RegionName;
                    }
                    else if (definitionAttribute != null)
                    {
                        regionName = $"{definitionAttribute.Type}/{definitionAttribute.Name}";
                    }
                    else
                    {
                        continue;
                    }
                    if (field.GetValue(null) is not PropertyMeta meta)
                        continue;
                    meta.RegisterNames(namespaceName, regionName);
                    var propertyName = meta.propertyName;

                    var fullName = PropertyKey.CombineName(namespaceName, regionName, propertyName);
                    if (!registries.TryGetKeyOfFullName(fullName, out PropertyKey key))
                    {
                        key = new PropertyKey(namespaceName, regionName, propertyName);
                        registries.RegisterFullName(fullName, key);
                    }
                    meta.SetRegisteredKey(key);
                }
            }
        }

        public static PropertyKey ConvertFromName(string propertyName)
        {
            if (registries.TryGetKeyOfFullName(propertyName, out var key))
            {
                return key;
            }
            PropertyKey.ParsePropertyName(propertyName, out var nsp, out var region, out var prop);
            key = new PropertyKey(nsp, region, prop);
            registries.RegisterFullName(propertyName, key);
            return key;
        }
        public static string ConvertToFullName(PropertyKey key)
        {
            if (registries.TryGetFullNameOfKey(key, out var name))
            {
                return name;
            }
            name = PropertyKey.CombineName(key.namespaceKey, key.regionKey, key.propertyKey);
            registries.RegisterFullName(name, key);
            return name;
        }

        private static Registries registries = new Registries();

        private class Registries
        {
            public void RegisterFullName(string fullName, PropertyKey key)
            {
                fullNameMap.Add(fullName, key);
                reversedFullNameMap.Add(key, fullName);
            }
            public bool TryGetFullNameOfKey(PropertyKey key, out string name)
            {
                return reversedFullNameMap.TryGetValue(key, out name);
            }
            public bool TryGetKeyOfFullName(string name, out PropertyKey key)
            {
                return fullNameMap.TryGetValue(name, out key);
            }
            private Dictionary<string, PropertyKey> fullNameMap = new Dictionary<string, PropertyKey>();
            private Dictionary<PropertyKey, string> reversedFullNameMap = new Dictionary<PropertyKey, string>();
        }
    }

}

