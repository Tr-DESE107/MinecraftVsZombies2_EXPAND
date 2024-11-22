using PVZEngine;

namespace MVZ2.Vanilla.Audios
{
    public static class VanillaMusicID
    {
        public readonly static NamespaceID mainmenu = Get("mainmenu");
        public readonly static NamespaceID choosing = Get("choosing");
        public readonly static NamespaceID day = Get("day");
        public readonly static NamespaceID halloween = Get("halloween");
        private static NamespaceID Get(string name)
        {
            return new NamespaceID(VanillaMod.spaceName, name);
        }
    }
}
