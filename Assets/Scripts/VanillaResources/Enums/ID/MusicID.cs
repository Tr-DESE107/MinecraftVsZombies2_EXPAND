using MVZ2.Vanilla;
using PVZEngine;

namespace MVZ2.GameContent
{
    public static class MusicID
    {
        public readonly static NamespaceID choosing = Get("choosing");
        public readonly static NamespaceID day = Get("day");
        private static NamespaceID Get(string name)
        {
            return new NamespaceID(Builtin.spaceName, name);
        }
    }
}
