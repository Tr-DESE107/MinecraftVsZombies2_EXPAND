using MVZ2.Vanilla;
using PVZEngine;

namespace MVZ2.GameContent
{
    public static class ArmorNames
    {
        public const string leatherCap = "leather_cap";
        public const string ironHelmet = "iron_helmet";
    }
    public static class ArmorID
    {
        public static readonly NamespaceID leatherCap = Get(ArmorNames.leatherCap);
        public static readonly NamespaceID ironHelmet = Get(ArmorNames.ironHelmet);
        private static NamespaceID Get(string name)
        {
            return new NamespaceID(VanillaMod.spaceName, name);
        }
    }
}
