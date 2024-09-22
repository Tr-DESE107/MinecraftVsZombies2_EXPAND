using System;
using PVZEngine;

namespace MVZ2.Resources
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
        public static bool TryParse(string str, string defaultNsp, out SpriteReference parsed)
        {
            parsed = null;
            var leftBracketIndex = str.LastIndexOf('[');
            var rightBracketIndex = str.LastIndexOf(']');
            if (leftBracketIndex < 0)
            {
                if (!NamespaceID.TryParse(str, defaultNsp, out var id))
                    return false;
                else
                {
                    parsed = new SpriteReference(id);
                    return true;
                }
            }
            else
            {
                if (leftBracketIndex >= rightBracketIndex)
                    return false;

                var bracketContent = str.Substring(leftBracketIndex + 1, rightBracketIndex - leftBracketIndex - 1);
                if (!int.TryParse(bracketContent, out var index))
                    return false;

                var idStr = str.Substring(0, leftBracketIndex);
                if (!NamespaceID.TryParse(idStr, defaultNsp, out var id))
                    return false;

                parsed = new SpriteReference(id, index);
                return true;
            }
        }
        public static SpriteReference Parse(string str, string defaultNsp)
        {
            if (TryParse(str, defaultNsp, out var parsed))
            {
                return parsed;
            }
            throw new FormatException($"Invalid SpriteReference {str}.");
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
