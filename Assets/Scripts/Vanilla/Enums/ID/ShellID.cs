using MVZ2.Vanilla;
using PVZEngine;

namespace MVZ2.GameContent
{
    public static class ShellNames
    {
        public const string flesh = "flesh";
        public const string stone = "stone";
        public const string grass = "grass";
    }
    public static class ShellID
    {
        public static readonly NamespaceID flesh = Get(ShellNames.flesh);
        public static readonly NamespaceID stone = Get(ShellNames.stone);
        public static readonly NamespaceID grass = Get(ShellNames.grass);
        private static NamespaceID Get(string name)
        {
            return new NamespaceID(VanillaMod.spaceName, name);
        }
    }
}
