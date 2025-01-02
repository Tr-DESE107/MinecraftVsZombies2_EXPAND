using MVZ2.Vanilla;
using PVZEngine;

namespace MVZ2.GameContent.Placements
{
    public static class VanillaPlacementNames
    {
        public const string any = "any";
        public const string normal = "normal";
        public const string buried = "buried";
        public const string aquatic = "aquatic";
        public const string pad = "pad";
        public const string dreamSilk = "dream_silk";
    }
    public static class VanillaPlacementID
    {
        public static readonly NamespaceID any = Get(VanillaPlacementNames.any);
        public static readonly NamespaceID normal = Get(VanillaPlacementNames.normal);
        public static readonly NamespaceID buried = Get(VanillaPlacementNames.buried);
        public static readonly NamespaceID aquatic = Get(VanillaPlacementNames.aquatic);
        public static readonly NamespaceID pad = Get(VanillaPlacementNames.pad);
        public static readonly NamespaceID dreamSilk = Get(VanillaPlacementNames.dreamSilk);
        private static NamespaceID Get(string name)
        {
            return new NamespaceID(VanillaMod.spaceName, name);
        }
    }
}
