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
            this.path = name;
        }
        public override int GetHashCode()
        {
            return spacename.GetHashCode() * 31 + path.GetHashCode();
        }
        public override bool Equals(object obj)
        {
            if (obj is NamespaceID otherRef)
            {
                return spacename == otherRef.spacename && path == otherRef.path;
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
        public static bool TryParse(string str, string defaultNsp, out NamespaceID parsed)
        {
            var colonIndex = str.IndexOf(':');
            string nsp;
            string path;
            parsed = null;
            if (colonIndex < 0)
            {
                // 没有冒号，使用默认命名空间。
                nsp = defaultNsp;
                path = str;
            }
            else if (colonIndex == 0)
            {
                // 冒号前没有内容，错误。
                return false;
            }
            else
            {
                // 冒号前有内容，获取命名空间和路径。
                nsp = str.Substring(0, colonIndex);
                path = str.Substring(colonIndex + 1);
            }
            if (!ValidateNamespace(nsp))
                return false;
            if (!ValidatePath(path))
                return false;
            parsed = new NamespaceID(nsp, path);
            return true;
        }
        public static NamespaceID Parse(string str, string defaultNsp)
        {
            if (TryParse(str, defaultNsp, out var parsed))
            {
                return parsed;
            }
            throw new FormatException($"Invalid NamespaceID {str}.");
        }
        public static bool ValidateNamespace(string nsp)
        {
            return !nsp.Contains(':') && !nsp.Contains('/');
        }
        public static bool ValidatePath(string path)
        {
            return !path.Contains(':');
        }
        public static bool IsValid(NamespaceID id)
        {
            if (id == null)
                return false;
            if (string.IsNullOrEmpty(id.spacename) || string.IsNullOrEmpty(id.path))
                return false;
            if (!ValidateNamespace(id.spacename) || !ValidatePath(id.path))
                return false;
            return true;
        }
        public override string ToString()
        {
            return $"{spacename}:{path}";
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
        public string path;

    }
}
