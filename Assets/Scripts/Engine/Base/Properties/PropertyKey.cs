using System;
using UnityEngine;

namespace PVZEngine
{
    public struct PropertyKey : IEquatable<PropertyKey>
    {
        public static readonly PropertyKey Invalid = new PropertyKey(0, 0);
        private int key;
        private const int PROPERTY_BITS = 20;
        private const int NAMESPACE_BITS = 12;

        private const int PROPERTY_KEY_SHIFT = 0;
        private const int PROPERTY_KEY_MASK = (1 << PROPERTY_BITS) - 1;
        private const int NAMESPACE_KEY_SHIFT = PROPERTY_KEY_SHIFT + PROPERTY_BITS;
        private const int NAMESPACE_KEY_MASK = ((1 << NAMESPACE_BITS) - 1) << NAMESPACE_KEY_SHIFT;
        public PropertyKey(int namespaceKey, int propertyKey)
        {
            key = ((propertyKey << PROPERTY_KEY_SHIFT) & PROPERTY_KEY_MASK) | 
                ((namespaceKey << NAMESPACE_KEY_SHIFT) & NAMESPACE_KEY_MASK);
        }

        public static bool IsValid(PropertyKey key)
        {
            return key.key > 0;
        }

        public override bool Equals(object obj)
        {
            return obj is PropertyKey key && Equals(key);
        }

        public override string ToString()
        {
            return key.ToString();
        }
        public override int GetHashCode()
        {
            return key;
        }
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
        public static string CombineName(string namespaceName, string regionName, string propertyName)
        {
            if (!string.IsNullOrEmpty(namespaceName))
            {
                if (!string.IsNullOrEmpty(regionName))
                {
                    return $"{namespaceName}:{regionName}/{propertyName}";
                }
                return $"{namespaceName}:{propertyName}";
            }
            else
            {
                if (!string.IsNullOrEmpty(regionName))
                {
                    return $"{regionName}/{propertyName}";
                }
                return propertyName;
            }
        }
        public static bool operator ==(PropertyKey lhs, PropertyKey rhs)
        {
            return lhs.key == rhs.key;
        }
        public static bool operator !=(PropertyKey lhs, PropertyKey rhs)
        {
            return !(lhs == rhs);
        }

        public bool Equals(PropertyKey other)
        {
            return key == other.key;
        }
    }
}
