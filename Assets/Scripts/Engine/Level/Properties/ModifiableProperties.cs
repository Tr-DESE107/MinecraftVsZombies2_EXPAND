using System;
using System.Collections.Generic;
using PVZEngine.Modifiers;
using Tools;

namespace PVZEngine.Level
{
    public class ModifiableProperties
    {
        public ModifiableProperties(IPropertyModifyTarget container)
        {
            Container = container;
        }
        public void SetProperty<T>(PropertyKey<T> name, T value)
        {
            if (properties.SetProperty(name, value))
            {
                UpdateModifiedProperty<T>(name);
                // 实体属性更改时，如果有利用该实体属性修改属性的修改器，更新一次该属性。
                var modifiersUsingThisProperty = Container.GetModifiersUsingProperty(name);
                if (modifiersUsingThisProperty != null && modifiersUsingThisProperty.Length > 0)
                {
                    foreach (var modifier in modifiersUsingThisProperty)
                    {
                        UpdateModifiedPropertyObject(modifier.PropertyName);
                    }
                }
            }
        }
        public void SetPropertyObject(IPropertyKey name, object value)
        {
            if (properties.SetPropertyObject(name, value))
            {
                UpdateModifiedPropertyObject(name);
                // 实体属性更改时，如果有利用该实体属性修改属性的修改器，更新一次该属性。
                var modifiersUsingThisProperty = Container.GetModifiersUsingProperty(name);
                if (modifiersUsingThisProperty != null && modifiersUsingThisProperty.Length > 0)
                {
                    foreach (var modifier in modifiersUsingThisProperty)
                    {
                        UpdateModifiedPropertyObject(modifier.PropertyName);
                    }
                }
            }
        }
        public bool TryGetPropertyObject(IPropertyKey name, out object result, bool ignoreBuffs = false)
        {
            if (!ignoreBuffs)
            {
                if (buffedProperties.TryGetPropertyObject(name, out var value))
                {
                    result = value;
                    return true;
                }
            }
            if (properties.TryGetPropertyObject(name, out var prop))
            {
                result = prop;
                return true;
            }
            if (fallbackCaches.TryGetValue(name, out var fallbackCache))
            {
                result = fallbackCache;
                return true;
            }
            if (Container.GetFallbackProperty(name, out var fallback))
            {
                AddFallbackCache(name, fallback);
                result = fallback;
                return true;
            }
            result = default;
            return false;
        }
        public bool TryGetProperty<T>(PropertyKey<T> name, out T result, bool ignoreBuffs = false)
        {
            if (TryGetPropertyObject(name, out var obj, ignoreBuffs))
            {
                if (obj.TryToGeneric<T>(out result))
                {
                    return true;
                }
            }
            result = default;
            return false;
        }
        public T GetProperty<T>(PropertyKey<T> name, bool ignoreBuffs = false)
        {
            if (TryGetProperty<T>(name, out var result, ignoreBuffs))
            {
                return result;
            }
            var value = name.DefaultValue;
            AddFallbackCache(name, value);
            return value;
        }
        public object GetPropertyObject(IPropertyKey name, bool ignoreBuffs = false)
        {
            if (TryGetPropertyObject(name, out var result, ignoreBuffs))
            {
                return result;
            }
            var value = name.DefaultValue;
            AddFallbackCache(name, value);
            return value;
        }
        public bool RemoveProperty(IPropertyKey name)
        {
            return properties.RemovePropertyObject(name);
        }
        public void AddFallbackCache(IPropertyKey key, object value)
        {
            fallbackCaches.Add(key, value);
        }
        public bool RemoveFallbackCache(IPropertyKey key)
        {
            return fallbackCaches.Remove(key);
        }
        public void ClearFallbackCaches()
        {
            fallbackCaches.Clear();
        }
        public IPropertyKey[] GetPropertyNames()
        {
            return properties.GetPropertyNames();
        }

        #region 增益
        public void UpdateAllModifiedProperties()
        {
            var paths = Container.GetModifiedProperties();
            foreach (var path in paths)
            {
                UpdateModifiedPropertyObject(path);
            }
        }
        public void UpdateModifiedPropertyObject(IPropertyKey name)
        {
            var baseValue = GetPropertyObject(name, ignoreBuffs: true);

            modifierContainerBuffer.Clear();
            Container.GetModifierItems(name, modifierContainerBuffer);

            var beforeValue = GetPropertyObject(name);
            var value = baseValue;
            if (modifierContainerBuffer.Count > 0)
            {
                value = modifierContainerBuffer.CalculateProperty(baseValue);
                buffedProperties.SetPropertyObject(name, value);
            }
            else
            {
                buffedProperties.RemovePropertyObject(name);
            }
            Container.UpdateModifiedProperty(name, beforeValue, value);
        }
        public void UpdateModifiedProperty<T>(PropertyKey<T> name)
        {
            var baseValue = GetProperty<T>(name, ignoreBuffs: true);

            modifierContainerBuffer.Clear();
            Container.GetModifierItems(name, modifierContainerBuffer);

            var beforeValue = GetProperty<T>(name);
            var value = baseValue;
            if (modifierContainerBuffer.Count > 0)
            {
                value = modifierContainerBuffer.CalculateProperty<T>(baseValue);
                buffedProperties.SetProperty(name, value);
            }
            else
            {
                buffedProperties.RemoveProperty(name);
            }
            Container.UpdateModifiedProperty(name, beforeValue, value);
        }
        #endregion
        public SerializableModifiableProperties ToSerializable()
        {
            return new SerializableModifiableProperties()
            {
                properties = properties.ToSerializable()
            };
        }
        public static ModifiableProperties FromSerializable(SerializableModifiableProperties seri, IPropertyModifyTarget container)
        {
            var block = new ModifiableProperties(container);
            block.properties = PropertyDictionary.FromSerializable(seri.properties);
            return block;
        }

        public IPropertyModifyTarget Container { get; }
        private PropertyDictionary properties = new PropertyDictionary();
        private PropertyDictionary buffedProperties = new PropertyDictionary();
        private Dictionary<IPropertyKey, object> fallbackCaches = new Dictionary<IPropertyKey, object>(new PropertyKeyComparer());
        private readonly Dictionary<Type, Action<object, object, object>> setPropertyDelegates = new();
        private readonly Dictionary<Type, Action<object, object>> updateModifiedPropertyDelegates = new();
        private List<ModifierContainerItem> modifierContainerBuffer = new List<ModifierContainerItem>();
        private delegate object UpdateModifiedPropertyDelegate(ModifiableProperties self, IPropertyKey key);
    }
    [Serializable]
    public class SerializableModifiableProperties
    {
        public SerializablePropertyDictionary properties;
    }
}
