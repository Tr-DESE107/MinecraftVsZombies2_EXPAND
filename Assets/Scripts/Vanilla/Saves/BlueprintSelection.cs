using System;
using PVZEngine;

namespace MVZ2.Vanilla.Saves
{
    [Serializable]
    public class BlueprintSelection
    {
        public BlueprintSelectionItem[] blueprints;
        public ArtifactSelectionItem[] artifacts;
    }
    [Serializable]
    public class BlueprintSelectionItem
    {
        public NamespaceID id;
        public bool isCommandBlock;

        public override bool Equals(object obj)
        {
            if (obj is not BlueprintSelectionItem item)
                return false;
            return this == item;
        }
        public override int GetHashCode()
        {
            var hashCode = id.GetHashCode();
            hashCode = (hashCode * 31) + isCommandBlock.GetHashCode();
            return hashCode;
        }
        public static bool operator ==(BlueprintSelectionItem lhs, BlueprintSelectionItem rhs)
        {
            if (lhs is null)
            {
                return rhs is null;
            }
            if (rhs is null)
            {
                return false;
            }
            return lhs.id == rhs.id && lhs.isCommandBlock == rhs.isCommandBlock;
        }
        public static bool operator !=(BlueprintSelectionItem lhs, BlueprintSelectionItem rhs)
        {
            return !(lhs == rhs);
        }
    }
    [Serializable]
    public class ArtifactSelectionItem
    {
        public NamespaceID id;
        public override bool Equals(object obj)
        {
            if (obj is not ArtifactSelectionItem item)
                return false;
            return this == item;
        }
        public override int GetHashCode()
        {
            return id.GetHashCode();
        }
        public static bool operator ==(ArtifactSelectionItem lhs, ArtifactSelectionItem rhs)
        {
            if (lhs is null)
            {
                return rhs is null;
            }
            if (rhs is null)
            {
                return false;
            }
            return lhs.id == rhs.id;
        }
        public static bool operator !=(ArtifactSelectionItem lhs, ArtifactSelectionItem rhs)
        {
            return !(lhs == rhs);
        }
    }
}
