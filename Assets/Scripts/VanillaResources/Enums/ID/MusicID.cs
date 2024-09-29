using PVZEngine;

namespace MVZ2.GameContent
{
    public static class MusicID
    {
        public readonly static NamespaceID mainmenu = Get("mainmenu");
        public readonly static NamespaceID choosing = Get("choosing");
        public readonly static NamespaceID day = Get("day");
        public readonly static NamespaceID halloween = Get("halloween");
        private static NamespaceID Get(string name)
        {
            return new NamespaceID(Builtin.spaceName, name);
        }
    }
}
