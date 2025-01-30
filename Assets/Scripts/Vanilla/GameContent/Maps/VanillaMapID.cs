using MVZ2.GameContent.Talk;
using MVZ2.Vanilla;
using PVZEngine;

namespace MVZ2.GameContent.Maps
{
    public static class VanillaMapID
    {
        public static readonly NamespaceID halloween = Get("halloween");
        public static readonly NamespaceID dream = Get("dream");
        public static readonly NamespaceID gensokyo = Get("gensokyo");
        public static readonly NamespaceID castle = Get("castle");
        private static NamespaceID Get(string name)
        {
            return new NamespaceID(VanillaMod.spaceName, name);
        }
    }
}
