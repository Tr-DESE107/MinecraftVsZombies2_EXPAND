using MVZ2.Vanilla;
using PVZEngine;

namespace MVZ2.GameContent
{
    public static class ShellNames
    {
        public const string leather = "leather";
        public const string flesh = "flesh";
        public const string bone = "bone";
        public const string stone = "stone";
        public const string grass = "grass";
        public const string metal = "metal";
    }
    public static class ShellID
    {
        public static readonly NamespaceID leather = Get(ShellNames.leather);
        public static readonly NamespaceID flesh = Get(ShellNames.flesh);
        public static readonly NamespaceID bone = Get(ShellNames.bone);
        public static readonly NamespaceID stone = Get(ShellNames.stone);
        public static readonly NamespaceID grass = Get(ShellNames.grass);
        public static readonly NamespaceID metal = Get(ShellNames.metal);
        private static NamespaceID Get(string name)
        {
            return new NamespaceID(VanillaMod.spaceName, name);
        }
    }
}
