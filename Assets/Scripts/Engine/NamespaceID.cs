namespace PVZEngine
{
    public class NamespaceID
    {
        public NamespaceID(string nsp, string name)
        {
            Namespace = nsp;
            Name = name;
        }
        public override int GetHashCode()
        {
            return Namespace.GetHashCode() * 31 + Name.GetHashCode();
        }
        public override bool Equals(object obj)
        {
            if (obj is NamespaceID otherRef)
            {
                return Namespace == otherRef.Namespace && Name == otherRef.Name;
            }
            return base.Equals(obj);
        }
        public override string ToString()
        {
            return $"{Namespace}:{Name}";
        }
        public static bool operator ==(NamespaceID lhs, NamespaceID rhs)
        {
            if (lhs == null)
                return rhs == null;
            return lhs.Equals(rhs);
        }
        public static bool operator !=(NamespaceID lhs, NamespaceID rhs)
        {
            return !(lhs == rhs);
        }
        public string Namespace { get; set; }
        public string Name { get; set; }

    }
}
