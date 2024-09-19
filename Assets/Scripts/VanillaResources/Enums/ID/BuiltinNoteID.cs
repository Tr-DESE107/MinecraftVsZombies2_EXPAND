using MVZ2.Vanilla;
using PVZEngine;

namespace MVZ2.GameContent
{
    public static class BuiltinNoteID
    {
        public static readonly NamespaceID help = Get("help");
        private static NamespaceID Get(string name)
        {
            return new NamespaceID(Builtin.spaceName, name);
        }
    }
}
