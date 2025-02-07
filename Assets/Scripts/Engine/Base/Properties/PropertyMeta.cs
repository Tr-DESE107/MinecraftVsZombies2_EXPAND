using System;

namespace PVZEngine
{
    public class PropertyMeta
    {
        public string namespaceName;
        public string regionName;
        public string propertyName;
        private PropertyKey key;
        public PropertyMeta(string name)
        {
            propertyName = name;
        }

        public override bool Equals(object obj)
        {
            return obj is PropertyMeta meta &&
                   namespaceName == meta.namespaceName &&
                   propertyName == meta.propertyName &&
                   regionName == meta.regionName;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(namespaceName, regionName, propertyName);
        }
        public override string ToString()
        {
            return PropertyKey.CombineName(namespaceName, regionName, propertyName);
        }

        public void RegisterNames(string namespaceName, string regionName)
        {
            this.namespaceName = namespaceName;
            this.regionName = regionName;
        }
        public void SetRegisteredKey(PropertyKey key)
        {
            this.key = key;
        }
        public static bool operator ==(PropertyMeta lhs, PropertyMeta rhs)
        {
            return lhs.namespaceName == rhs.namespaceName && lhs.regionName == rhs.regionName && lhs.propertyName == rhs.propertyName;
        }
        public static bool operator !=(PropertyMeta lhs, PropertyMeta rhs)
        {
            return lhs.namespaceName != rhs.namespaceName || lhs.regionName == rhs.regionName || lhs.propertyName != rhs.propertyName;
        }
        public static implicit operator PropertyKey(PropertyMeta meta)
        {
            return meta.key;
        }
    }
}
