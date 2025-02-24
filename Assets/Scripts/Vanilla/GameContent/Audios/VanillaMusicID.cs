using PVZEngine;

namespace MVZ2.Vanilla.Audios
{
    public static class VanillaMusicID
    {
        public readonly static NamespaceID mainmenu = Get("mainmenu");
        public readonly static NamespaceID choosing = Get("choosing");
        public readonly static NamespaceID day = Get("day");
        public readonly static NamespaceID halloween = Get("halloween");
        public readonly static NamespaceID halloweenBoss = Get("halloween_boss");
        public readonly static NamespaceID dreamLevel = Get("dream_level");
        public readonly static NamespaceID nightmareLevel = Get("nightmare_level");
        public readonly static NamespaceID nightmareBoss = Get("nightmare_boss");
        public readonly static NamespaceID nightmareBoss2 = Get("nightmare_boss2");
        public readonly static NamespaceID seija = Get("seija");
        public readonly static NamespaceID witherBoss = Get("wither_boss");
        private static NamespaceID Get(string name)
        {
            return new NamespaceID(VanillaMod.spaceName, name);
        }
    }
}
