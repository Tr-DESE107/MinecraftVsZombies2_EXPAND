using System;

namespace PVZEngine
{
    public abstract class PropertyMeta
    {
        public string namespaceName;
        public string regionName;
        public string propertyName;
        public Type propertyType;
        public PropertyMeta(string name, Type propertyType)
        {
            propertyName = name;
            this.propertyType = propertyType;
        }
        public override string ToString()
        {
            return PropertyKeyHelper.CombineName(namespaceName, regionName, propertyName);
        }
        public abstract void SetRegisteredKey(IPropertyKey key);

        public void RegisterNames(string namespaceName, string regionName)
        {
            this.namespaceName = namespaceName;
            this.regionName = regionName;
        }
    }
    public class PropertyMeta<T> : PropertyMeta
    {
        private PropertyKey<T> key;
        public PropertyMeta(string name) : base(name, typeof(T))
        {
        }

        public override void SetRegisteredKey(IPropertyKey key)
        {
            if (key is PropertyKey<T> tKey)
            {
                this.key = tKey;
            }
        }
        public override bool Equals(object obj)
        {
            if (obj is IPropertyKey key)
            {
                return this.key.Equals(key);
            }
            return obj is PropertyMeta<T> meta &&
                   namespaceName == meta.namespaceName &&
                   propertyName == meta.propertyName &&
                   regionName == meta.regionName;
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(namespaceName, regionName, propertyName);
        }
        public static bool operator ==(PropertyMeta<T> lhs, PropertyMeta<T> rhs)
        {
            return lhs.namespaceName == rhs.namespaceName && lhs.regionName == rhs.regionName && lhs.propertyName == rhs.propertyName;
        }
        public static bool operator !=(PropertyMeta<T> lhs, PropertyMeta<T> rhs)
        {
            return lhs.namespaceName != rhs.namespaceName || lhs.regionName == rhs.regionName || lhs.propertyName != rhs.propertyName;
        }
        public static implicit operator PropertyKey<T>(PropertyMeta<T> meta)
        {
            return meta.key;
        }
    }
}
