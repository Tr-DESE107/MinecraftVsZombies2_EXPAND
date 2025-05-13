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
                        if (!string.IsNullOrEmpty(fieldAttribute.TypeName))
                        {
                            regionName = $"{fieldAttribute.TypeName}/{fieldAttribute.RegionName}";
                        }
                        else
                        {
                            regionName = $"{fieldAttribute.RegionName}";
                        }
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
                    if (registries.TryGetKeyOfFullName(fullName, out PropertyKey key))
                    {
                        Debug.LogWarning($"Duplicate property meta {meta}");
                       
                    }
                    else
                    {
                        string propName;
                        if (!string.IsNullOrEmpty(regionName))
                        {
                            propName = $"{regionName}/{propertyName}";
                        }
                        else
                        {
                            propName = propertyName;
                        }
                        registries.GetOrRegisterPropertyKey(namespaceName, propName, out var namespaceKey, out var propertyKey);
                        key = new PropertyKey(namespaceKey, propertyKey);
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
            Debug.LogWarning($"Property with name {propertyName} is not registered.");
            return PropertyKey.Invalid;
        }
        public static PropertyKey ConvertFromName(string propertyName, string regionName, string defaultNsp)
        {
            var id = NamespaceID.Parse(propertyName, defaultNsp);
            var newName = PropertyKey.CombineName(id.SpaceName, regionName, id.Path);
            if (registries.TryGetKeyOfFullName(newName, out var key))
            {
                return key;
            }
            Debug.LogWarning($"Property with name {newName} is not registered.");
            return PropertyKey.Invalid;
        }
        public static string ConvertToFullName(PropertyKey key)
        {
            if (registries.TryGetFullNameOfKey(key, out var name))
            {
                return name;
            }
            Debug.LogWarning($"Cannot find property name of key {key}.");
            return null;
        }

        private static Registries registries = new Registries();

        private class Registries
        {
            public NamespaceRegistry GetOrRegisterNamespace(string namespaceName)
            {
                if (string.IsNullOrEmpty(namespaceName))
                {
                    return emptyNamespace;
                }
                if (registeredNamespaces.TryGetValue(namespaceName, out var value))
                {
                    return value;
                }
                currentNamespaceNumber++;
                var registry = new NamespaceRegistry(currentNamespaceNumber);
                registeredNamespaces.Add(namespaceName, registry);
                return registry;
            }
            public void GetOrRegisterPropertyKey(string namespaceName, string propertyName, out int namespaceKey, out int propertyKey)
            {
                var namespaceRegistry = GetOrRegisterNamespace(namespaceName);
                namespaceKey = namespaceRegistry.number;
                propertyKey = namespaceRegistry.GetOrRegisterPropertyKey(propertyName);
            }
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
            private int currentNamespaceNumber = 0;
            private NamespaceRegistry emptyNamespace = new NamespaceRegistry(0);
            private Dictionary<string, NamespaceRegistry> registeredNamespaces = new Dictionary<string, NamespaceRegistry>();
            private Dictionary<string, PropertyKey> fullNameMap = new Dictionary<string, PropertyKey>();
            private Dictionary<PropertyKey, string> reversedFullNameMap = new Dictionary<PropertyKey, string>();
        }
        private class NamespaceRegistry
        {
            public NamespaceRegistry(int number)
            {
                this.number = number;
            }
            public int GetOrRegisterPropertyKey(string propertyName)
            {
                if (string.IsNullOrEmpty(propertyName))
                {
                    return 0;
                }
                if (registeredProperties.TryGetValue(propertyName, out var value))
                {
                    return value;
                }
                currentPropertyNumber++;
                var number = currentPropertyNumber;
                registeredProperties.Add(propertyName, number);
                return number;
            }
            private int currentPropertyNumber = 0;
            public int number;
            public Dictionary<string, int> registeredProperties = new Dictionary<string, int>();
        }
    }

}

