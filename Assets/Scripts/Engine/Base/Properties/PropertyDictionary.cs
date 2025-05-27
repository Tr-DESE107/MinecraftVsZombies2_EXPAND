using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace PVZEngine
{
    public class PropertyDictionary
    {
        public bool SetProperty<T>(PropertyKey<T> key, T value)
        {
            var typedDict = GetTypedDict<T>();
            if (value == null)
            {
                if (!typedDict.TryGetValue(key, out var valueBefore) || valueBefore == null)
                    return false;
            }
            else
            {
                if (typedDict.TryGetValue(key, out var valueBefore) && value.Equals(valueBefore))
                    return false;
            }
            typedDict[key] = value;
            propertyKeys.Add(key);
            return true;
        }
        public bool SetPropertyObject(IPropertyKey name, object value)
        {
            var valueType = name.Type;
            if (!DelegateCaches.TryGetDelegate<SetPropertyObjectDelegate>(valueType, out var dele))
            {
                var selfType = GetType();
                var propertyKeyType = typeof(PropertyKey<>).MakeGenericType(valueType);
                var delegateType = typeof(Func<,,,>).MakeGenericType(selfType, propertyKeyType, valueType, typeof(bool));
                dele = DelegateCaches.AddDelegate<SetPropertyObjectDelegate>(
                    valueType,
                    this,
                    nameof(SetProperty),
                    new Type[] { propertyKeyType, valueType },
                    delegateType,
                    d => (self, name, value) => (bool)d?.DynamicInvoke(self, name, value)
                );
            }
            return dele(this, name, value);
        }
        public T GetProperty<T>(PropertyKey<T> name)
        {
            return TryGetProperty<T>(name, out var value) ? value : default;
        }
        public object GetPropertyObject(IPropertyKey name)
        {
            var valueType = name.Type;
            if (!DelegateCaches.TryGetDelegate<GetPropertyObjectDelegate>(valueType, out var dele))
            {
                var selfType = GetType();
                var propertyKeyType = typeof(PropertyKey<>).MakeGenericType(valueType);
                var delegateType = typeof(Func<,,>).MakeGenericType(selfType, propertyKeyType, valueType);
                dele = DelegateCaches.AddDelegate<GetPropertyObjectDelegate>(
                    valueType,
                    this,
                    nameof(GetProperty),
                    new Type[] { propertyKeyType },
                    delegateType,
                    d => (self, n) => d?.DynamicInvoke(self, n));
            }
            return dele(this, name);
        }
        public bool ContainsProperty(IPropertyKey name)
        {
            return propertyKeys.Contains(name);
        }
        public bool TryGetProperty<T>(PropertyKey<T> key, out T value)
        {
            var dict = GetTypedDict<T>();
            if (dict.TryGetValue(key, out value))
                return true;
            value = default;
            return false;
        }
        public bool RemoveProperty<T>(PropertyKey<T> name)
        {
            bool removed = false;
            foreach (var dict in typedDicts)
            {
                if (dict.Value.Contains(name))
                {
                    dict.Value.Remove(name);
                    removed = true;
                }
            }
            if (removed)
            {
                propertyKeys.Remove(name);
            }
            return removed;
        }
        public IPropertyKey[] GetPropertyNames()
        {
            return propertyKeys.ToArray();
        }
        public SerializablePropertyDictionary ToSerializable()
        {
            var properties = new Dictionary<string, object>();
            foreach (var p in typedDicts)
            {
                var dict = p.Value;
                foreach (var key in dict.Keys)
                {
                    var propKey = (IPropertyKey)key;
                    var propName = PropertyMapper.ConvertToFullName(propKey);
                    if (string.IsNullOrEmpty(propName))
                    {
                        Debug.LogWarning($"Trying to serialize a property with key {propKey}, which is not registered.");
                        continue;
                    }
                    if (properties.ContainsKey(propName))
                    {
                        Debug.LogWarning($"A same property with name {propName} has already been added to the property dictionary.");
                        continue;
                    }
                    var value = dict[key];
                    properties.Add(propName, value);
                }
            }
            return new SerializablePropertyDictionary()
            {
                properties = properties
            };
        }
        public static PropertyDictionary FromSerializable(SerializablePropertyDictionary seri)
        {
            var dict = new PropertyDictionary();
            if (seri.properties != null)
            {
                foreach (var pair in seri.properties)
                {
                    var key = PropertyMapper.ConvertFromName(pair.Key);
                    var value = pair.Value;
                    var typedDict = dict.GetTypedDict(key.Type);
                    typedDict.Add(key, value);
                }
            }
            return dict;
        }
        private Dictionary<PropertyKey<T>, T> GetTypedDict<T>()
        {
            var t = typeof(T);
            if (!typedDicts.TryGetValue(t, out var dict))
            {
                dict = new Dictionary<PropertyKey<T>, T>(8);
                typedDicts.Add(t, dict);
            }
            return (Dictionary<PropertyKey<T>, T>)dict;
        }
        private IDictionary GetTypedDict(Type valueType)
        {
            // 空 type 当成 object 处理
            if (valueType == null)
                valueType = typeof(object);

            if (!typedDicts.TryGetValue(valueType, out var dictObj))
            {
                // 1) 拿到开泛型 Dictionary<,>
                var genericDictDef = typeof(Dictionary<,>);

                // 2) 构造具体类型 Dictionary<PropertyKey, valueType>
                var propertyKeyType = typeof(PropertyKey<>).MakeGenericType(valueType);
                var concreteDictType = genericDictDef.MakeGenericType(propertyKeyType, valueType);

                // 3) 用 capacity=8 调用 ctor
                dictObj = (IDictionary)Activator.CreateInstance(concreteDictType, 8);

                // 4) 缓存
                typedDicts[valueType] = dictObj;
            }

            // Dictionary<PropertyKey, T> 实现了非泛型 IDictionary
            return (IDictionary)dictObj;
        }
        public int Count => propertyKeys.Count;
        private HashSet<IPropertyKey> propertyKeys = new HashSet<IPropertyKey>();
        private Dictionary<Type, IDictionary> typedDicts = new Dictionary<Type, IDictionary>(4);
        private delegate bool SetPropertyObjectDelegate(PropertyDictionary self, IPropertyKey name, object value);
        private delegate object GetPropertyObjectDelegate(PropertyDictionary self, IPropertyKey name);
    }
    [Serializable]
    public class SerializablePropertyDictionary
    {
        public Dictionary<string, object> properties;
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
