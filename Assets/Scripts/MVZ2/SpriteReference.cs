using System;
using PVZEngine;
using UnityEngine.AddressableAssets;

namespace MVZ2
{
    [Serializable]
    public class SpriteReference
    {
        public SpriteReference(NamespaceID id)
        {
            this.id = id;
        }
        public SpriteReference(NamespaceID id, int index)
        {
            this.id = id;
            isSheet = true;
            this.index = index;
        }
        public override int GetHashCode()
        {
            var hash = id.GetHashCode();
            hash = hash * 31 + isSheet.GetHashCode();
            hash = hash * 31 + index.GetHashCode();
            return hash;
        }
        public override bool Equals(object obj)
        {
            if (obj is SpriteReference otherRef)
            {
                return otherRef.id == id && otherRef.isSheet == isSheet && otherRef.index == index;
            }
            return base.Equals(obj);
        }
        public override string ToString()
        {
            return $"{id}{(isSheet ? $"[{index}]" : string.Empty)}";
        }
        public static bool operator ==(SpriteReference lhs, SpriteReference rhs)
        {
            if (lhs is null)
                return rhs is null;
            return lhs.Equals(rhs);
        }
        public static bool operator !=(SpriteReference lhs, SpriteReference rhs)
        {
            return !(lhs == rhs);
        }
        public NamespaceID id;
        public bool isSheet;
        public int index;
    }
}
