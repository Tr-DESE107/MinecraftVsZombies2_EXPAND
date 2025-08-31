using PVZEngine.Level;

namespace PVZEngine.Entities
{
    public class NullSourceReference : ILevelSourceReference
    {
        public NullSourceReference()
        {

        }
        public NullSourceReference Clone()
        {
            return new NullSourceReference();
        }
        public override bool Equals(object obj)
        {
            if (obj is NullSourceReference entityRef)
            {
                return true;
            }
            return base.Equals(obj);
        }
        public override int GetHashCode()
        {
            return ID.GetHashCode();
        }
        public static bool operator ==(NullSourceReference lhs, NullSourceReference rhs)
        {
            if (lhs is null)
                return rhs is null;
            return lhs.Equals(rhs);
        }
        public static bool operator !=(NullSourceReference lhs, NullSourceReference rhs)
        {
            return !(lhs == rhs);
        }
        ILevelSourceReference ILevelSourceReference.Clone()
        {
            return Clone();
        }
        public ILevelSourceTarget GetTarget(LevelEngine level) => null;
        public ILevelSourceReference Parent => null;
        public int Faction => 0;
        public NamespaceID DefinitionID => null;
        public long ID => 0;
    }
}