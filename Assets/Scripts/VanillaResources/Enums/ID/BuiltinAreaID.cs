using MVZ2.Vanilla;
using PVZEngine;

namespace MVZ2.GameContent
{
    public static class BuiltinAreaID
    {
        public static readonly NamespaceID day = Get("day");
        private static NamespaceID Get(string name)
        {
            return new NamespaceID(Builtin.spaceName, name);
        }
    }
}
