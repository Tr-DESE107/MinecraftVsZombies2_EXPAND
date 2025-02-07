using System;
using UnityEngine;

namespace PVZEngine
{
    public struct PropertyKey : IEquatable<PropertyKey>
    {
        public string namespaceKey;
        public string regionKey;
        public string propertyKey;
        public static readonly PropertyKey Invalid = new PropertyKey(null, null, null);
        public PropertyKey(string namespaceKey, string regionKey, string propertyKey)
        {
            this.namespaceKey = namespaceKey;
            this.regionKey = regionKey;
            this.propertyKey = propertyKey;
        }

        public static bool IsValid(PropertyKey key)
        {
            return !string.IsNullOrEmpty(key.propertyKey);
        }

        public override bool Equals(object obj)
        {
            return obj is PropertyKey key && Equals(key);
        }

        public override string ToString()
        {
            return $"{namespaceKey}:{regionKey}/{propertyKey}";
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(namespaceKey, regionKey, propertyKey);
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
            return lhs.namespaceKey == rhs.namespaceKey && lhs.regionKey == rhs.regionKey && lhs.propertyKey == rhs.propertyKey;
        }
        public static bool operator !=(PropertyKey lhs, PropertyKey rhs)
        {
            return !(lhs == rhs);
        }

        public bool Equals(PropertyKey other)
        {
            return namespaceKey == other.namespaceKey &&
                   regionKey == other.regionKey &&
                   propertyKey == other.propertyKey;
        }
    }
}
