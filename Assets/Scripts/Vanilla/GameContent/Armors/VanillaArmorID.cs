using MVZ2.Vanilla;
using PVZEngine;

namespace MVZ2.GameContent.Armors
{
    public static class VanillaArmorNames
    {
        public const string leatherCap = "leather_cap";
        public const string ironHelmet = "iron_helmet";
        public const string mesmerizerCrown = "mesmerizer_crown";
        public const string berserkerHelmet = "berserker_helmet";
    }
    public static class VanillaArmorID
    {
        public static readonly NamespaceID leatherCap = Get(VanillaArmorNames.leatherCap);
        public static readonly NamespaceID ironHelmet = Get(VanillaArmorNames.ironHelmet);
        public static readonly NamespaceID mesmerizerCrown = Get(VanillaArmorNames.mesmerizerCrown);
        public static readonly NamespaceID bersekerHelmet = Get(VanillaArmorNames.berserkerHelmet);
        private static NamespaceID Get(string name)
        {
            return new NamespaceID(VanillaMod.spaceName, name);
        }
    }
}
