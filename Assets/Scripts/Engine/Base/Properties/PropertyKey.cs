using System;
using System.Collections.Generic;

namespace PVZEngine
{
    public interface IPropertyKey
    {
        int Key { get; }
        Type Type { get; }
        object DefaultValue { get; }
    }
    public static class PropertyKeyHelper
    {
        public static void ParsePropertyName(string text, out string nsp, out string region, out string property)
        {
            var colon = text.IndexOf(':');
            var slash = text.LastIndexOf("/");
            if (colon >= 0)
            {
                nsp = text.Substring(0, colon);
                if (slash >= 0)
                {
                    region = text.Substring(colon + 1, slash);
                    property = text.Substring(slash + 1);
                }
                else
                {
                    region = null;
                    property = text.Substring(colon + 1);
                }
            }
            else
            {
                nsp = null;
                if (slash >= 0)
                {
                    region = text.Substring(0, slash);
                    property = text.Substring(slash + 1);
                }
                else
                {
                    region = null;
                    property = text;
                }
            }
        }
        public static string CombineFullName(string namespaceName, params string[] names)
        {
            var laterName = CombineRegionName(names);
            if (!string.IsNullOrEmpty(namespaceName))
            {
                return $"{namespaceName}:{laterName}";
            }
            return laterName;
        }
        public static string CombineRegionName(params string[] names)
        {
            return string.Join("/", names);
        }
        public static string ParsePropertyFullName(string propertyName, string defaultNsp, string regionName = null)
        {
            var propID = NamespaceID.Parse(propertyName, defaultNsp);
            if (string.IsNullOrEmpty(regionName))
            {
                return PropertyKeyHelper.CombineFullName(propID.SpaceName, propID.Path);
            }
            return PropertyKeyHelper.CombineFullName(propID.SpaceName, regionName, propID.Path);
        }
        public static IPropertyKey FromType(int namespaceKey, int propertyKey, Type propertyType, object defaultValue)
        {
            var type = typeof(PropertyKey<>).MakeGenericType(propertyType);
            return (IPropertyKey)Activator.CreateInstance(type, namespaceKey, propertyKey, defaultValue);
        }
        public static bool IsValid(this IPropertyKey key)
        {
            return key.Key > 0;
        }
        public static readonly IPropertyKey Invalid = new InvalidPropertyKey();
    }
    public struct InvalidPropertyKey : IPropertyKey
    {
        int IPropertyKey.Key => 0;
        Type IPropertyKey.Type => typeof(object);
        object IPropertyKey.DefaultValue => null;
    }
    public struct PropertyKey<T> : IPropertyKey
    {
        int IPropertyKey.Key => key;
        Type IPropertyKey.Type => typeof(T);
        object IPropertyKey.DefaultValue => DefaultValue;
        public T DefaultValue { get; }
        private int key;
        private const int PROPERTY_BITS = 20;
        private const int NAMESPACE_BITS = 12;

        private const int PROPERTY_KEY_SHIFT = 0;
        private const int PROPERTY_KEY_MASK = (1 << PROPERTY_BITS) - 1;
        private const int NAMESPACE_KEY_SHIFT = PROPERTY_KEY_SHIFT + PROPERTY_BITS;
        private const int NAMESPACE_KEY_MASK = ((1 << NAMESPACE_BITS) - 1) << NAMESPACE_KEY_SHIFT;
        public PropertyKey(int namespaceKey, int propertyKey, T defaultValue)
        {
            key = ((propertyKey << PROPERTY_KEY_SHIFT) & PROPERTY_KEY_MASK) |
                ((namespaceKey << NAMESPACE_KEY_SHIFT) & NAMESPACE_KEY_MASK);
            DefaultValue = defaultValue;
        }
        public override bool Equals(object obj)
        {
            return obj is PropertyKey<T> key && Equals(key);
        }
        public override string ToString()
        {
            return key.ToString();
        }
        public override int GetHashCode()
        {
            return key;
        }
        public static bool operator ==(PropertyKey<T> lhs, IPropertyKey rhs)
        {
            return lhs.key == rhs.Key;
        }
        public static bool operator !=(PropertyKey<T> lhs, IPropertyKey rhs)
        {
            return !(lhs == rhs);
        }
        public static bool operator ==(IPropertyKey lhs, PropertyKey<T> rhs)
        {
            return lhs.Key == rhs.key;
        }
        public static bool operator !=(IPropertyKey lhs, PropertyKey<T> rhs)
        {
            return !(lhs == rhs);
        }

        public bool Equals(PropertyKey<T> other)
        {
            return key == other.key;
        }
        public bool Equals(IPropertyKey other)
        {
            return key == other.Key;
        }
    }
    public class PropertyKeyComparer : IEqualityComparer<IPropertyKey>
    {
        public bool Equals(IPropertyKey x, IPropertyKey y)
        {
            if (ReferenceEquals(x, y))
                return true;
            if (x is null || y is null)
                return false;
            return x.Key == y.Key; // 直接比较 int Key
        }

        public int GetHashCode(IPropertyKey obj)
        {
            return obj.Key; // 直接使用 int 的哈希码
        }
    }
}
