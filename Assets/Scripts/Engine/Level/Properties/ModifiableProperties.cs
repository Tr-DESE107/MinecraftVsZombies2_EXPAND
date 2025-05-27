using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using PVZEngine.Modifiers;
using Tools;
using UnityEngine;

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
                fallbackCaches.Add(name, fallback);
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
        public T GetProperty<T>(PropertyKey<T> name, bool ignoreBuffs = false) => TryGetProperty<T>(name, out var result, ignoreBuffs) ? result : default;
        public object GetPropertyObject(IPropertyKey name, bool ignoreBuffs = false) => TryGetPropertyObject(name, out var result, ignoreBuffs) ? result : null;
        public bool RemoveProperty(IPropertyKey name)
        {
            return properties.RemovePropertyObject(name);
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
            var valueType = name.Type;
            if (!DelegateCaches.TryGetDelegate<UpdateModifiedPropertyDelegate>(valueType, out var dele))
            {
                var selfType = GetType();
                var propertyKeyType = typeof(PropertyKey<>).MakeGenericType(valueType);
                var delegateType = typeof(Action<,>).MakeGenericType(selfType, propertyKeyType);
                dele = DelegateCaches.AddDelegate<UpdateModifiedPropertyDelegate>(
                    valueType,
                    this,
                    nameof(UpdateModifiedProperty),
                    new Type[] { propertyKeyType },
                    delegateType,
                    d => (self, name) => d?.DynamicInvoke(self, name));
            }
            dele(this, name);
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
        private Dictionary<IPropertyKey, object> fallbackCaches = new Dictionary<IPropertyKey, object>();
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
    public static class DelegateCaches
    {
        public static bool TryGetDelegate<T>(Type genericType, out T del) where T : Delegate
        {
            var dict = GetDelegateDictionary<T>();
            return dict.TryGetValue(genericType, out del);
        }
        public static T AddDelegate<T>(Type genericType, object obj, string methodName, Type[] argumentTypes, Type delegateType, Func<Delegate, T> delegateGetter) where T : Delegate
        {
            var dict = GetDelegateDictionary<T>();
            var objType = obj.GetType();
            MethodInfo method = GetGenericMethod(objType, methodName, argumentTypes);

            // 创建委托
            var methodGeneric = method.MakeGenericMethod(genericType);
            var d = Delegate.CreateDelegate(delegateType, null, methodGeneric);

            // 包装成统一的Action<object, object, object>
            T wrapped = delegateGetter.Invoke(d);

            dict[genericType] = wrapped;
            return wrapped;
        }
        private static MethodInfo GetGenericMethod(Type objType, string methodName, Type[] argumentTypes)
        {
            foreach (var m in objType.GetMethods())
            {
                if (m.Name != methodName || !m.IsGenericMethod)
                    continue;
                var parameters = m.GetParameters();
                if (parameters.Length != argumentTypes.Length)
                    continue;
                bool match = true;
                for (int i = 0; i < parameters.Length; i++)
                {
                    if (parameters[i].ParameterType.IsGenericParameter)
                        continue; // 泛型参数跳过
                    if (parameters[i].ParameterType.IsGenericType &&
                        argumentTypes[i].IsGenericType &&
                        parameters[i].ParameterType.GetGenericTypeDefinition() == argumentTypes[i].GetGenericTypeDefinition())
                        continue;
                    if (parameters[i].ParameterType != argumentTypes[i])
                    {
                        match = false;
                        break;
                    }
                }
                if (match)
                {
                    return m;
                }
            }
            return null;
        }
        private static Dictionary<Type, T> GetDelegateDictionary<T>() where T : Delegate
        {
            var t = typeof(T);
            if (!typedDicts.TryGetValue(t, out var dict))
            {
                dict = new Dictionary<Type, T>();
                typedDicts.Add(t, dict);
            }
            return (Dictionary<Type, T>)dict;
        }
        private static readonly Dictionary<Type, IDictionary> typedDicts = new();
    }
}
