using System;
using System.Text;

namespace PVZEngine
{
    [Serializable]
    public class NamespaceID
    {
        public NamespaceID(string nsp, string name)
        {
            spacename = nsp;
            this.name = name;
        }
        public override int GetHashCode()
        {
            return spacename.GetHashCode() * 31 + name.GetHashCode();
        }
        public override bool Equals(object obj)
        {
            if (obj is NamespaceID otherRef)
            {
                return spacename == otherRef.spacename && name == otherRef.name;
            }
            return base.Equals(obj);
        }
        public static string ConvertName(string text)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < text.Length; i++)
            {
                char chr = text[i];
                if (!char.IsDigit(chr) && !char.IsLetter(chr))
                    sb.Append('_');
                else
                {
                    if (char.IsUpper(chr) && i > 0)
                        sb.Append('_');
                    sb.Append(char.ToLower(chr));
                }
            }
            return sb.ToString();
        }
        public override string ToString()
        {
            return $"{spacename}:{name}";
        }
        public static bool operator ==(NamespaceID lhs, NamespaceID rhs)
        {
            if (lhs is null)
                return rhs is null;
            return lhs.Equals(rhs);
        }
        public static bool operator !=(NamespaceID lhs, NamespaceID rhs)
        {
            return !(lhs == rhs);
        }
        public string spacename;
        public string name;

    }
}
