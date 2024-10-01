using MVZ2.Vanilla;
using PVZEngine;

namespace MVZ2.GameContent
{
    public static class VanillaShellNames
    {
        public const string leather = "leather";
        public const string flesh = "flesh";
        public const string bone = "bone";
        public const string stone = "stone";
        public const string grass = "grass";
        public const string metal = "metal";
    }
    public static class VanillaShellID
    {
        public static readonly NamespaceID leather = Get(VanillaShellNames.leather);
        public static readonly NamespaceID flesh = Get(VanillaShellNames.flesh);
        public static readonly NamespaceID bone = Get(VanillaShellNames.bone);
        public static readonly NamespaceID stone = Get(VanillaShellNames.stone);
        public static readonly NamespaceID grass = Get(VanillaShellNames.grass);
        public static readonly NamespaceID metal = Get(VanillaShellNames.metal);
        private static NamespaceID Get(string name)
        {
            return new NamespaceID(VanillaMod.spaceName, name);
        }
    }
}
