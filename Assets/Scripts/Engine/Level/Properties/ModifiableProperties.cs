using System;
using System.Collections.Generic;
using PVZEngine.Modifiers;
using Tools;
using static UnityEngine.Networking.UnityWebRequest;

namespace PVZEngine.Level
{
    public class ModifiableProperties
    {
        public ModifiableProperties(IPropertyModifyTarget container)
        {
            Container = container;
        }
        public void SetProperty(string name, object value)
        {
            if (properties.SetProperty(name, value))
            {
                UpdateModifiedProperty(name);
                // 实体属性更改时，如果有利用该实体属性修改属性的修改器，更新一次该属性。
                var modifiersUsingThisProperty = Container.GetModifiersUsingProperty(name);
                if (modifiersUsingThisProperty != null && modifiersUsingThisProperty.Length > 0)
                {
                    foreach (var modifier in modifiersUsingThisProperty)
                    {
                        UpdateModifiedProperty(modifier.PropertyName);
                    }
                }
            }
        }
        public object GetProperty(string name, bool ignoreBuffs = false)
        {
            if (TryGetProperty(name, out var result, ignoreBuffs))
            {
                return result;
            }
            return null;
        }
        public bool TryGetProperty(string name, out object result, bool ignoreBuffs = false)
        {
            if (!ignoreBuffs)
            {
                if (buffedProperties.TryGetProperty(name, out var value))
                {
                    result = value;
                    return true;
                }
            }
            if (properties.TryGetProperty(name, out var prop))
            {
                result = prop;
                return true;
            }

            if (Container.GetFallbackProperty(name, out var fallback))
            {
                result = fallback;
                return true;
            }
            result = null;
            return false;
        }
        public T GetProperty<T>(string name, bool ignoreBuffs = false)
        {
            if (TryGetProperty<T>(name, out var result, ignoreBuffs))
            {
                return result;
            }
            return result;
        }
        public bool TryGetProperty<T>(string name, out T value, bool ignoreBuffs = false)
        {
            if (TryGetProperty(name, out object prop, ignoreBuffs))
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
            return properties.RemoveProperty(name);
        }
        public string[] GetPropertyNames()
        {
            return properties.GetPropertyNames();
        }

        #region 增益
        public void UpdateAllModifiedProperties()
        {
            var paths = Container.GetModifiedProperties();
            foreach (var path in paths)
            {
                UpdateModifiedProperty(path);
            }
        }
        public void UpdateModifiedProperty(string name)
        {
            var baseValue = GetProperty(name, ignoreBuffs: true);

            modifierContainerBuffer.Clear();
            Container.GetModifierItems(name, modifierContainerBuffer);

            var value = baseValue;
            if (modifierContainerBuffer.Count > 0)
            {
                value = modifierContainerBuffer.CalculateProperty(baseValue);
                buffedProperties.SetProperty(name, value);
            }
            else
            {
                buffedProperties.RemoveProperty(name);
            }
            Container.UpdateModifiedProperty(name, value);
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
        private List<ModifierContainerItem> modifierContainerBuffer = new List<ModifierContainerItem>();
    }
    [Serializable]
    public class SerializableModifiableProperties
    {
        public SerializablePropertyDictionary properties;
    }
}
