using PVZEngine;

namespace MVZ2.Vanilla.Grids
{
    public static class VanillaGridLayers
    {
        public static readonly NamespaceID main = Get("main");
        public static readonly NamespaceID carrier = Get("carrier");
        public static readonly NamespaceID protector = Get("protector");
        public static readonly NamespaceID tool = Get("tool");

        public const int GROUP_PROTECTOR = 1;
        public const int GROUP_MAIN = 0;
        public const int GROUP_CARRIER = -1;

        private static NamespaceID Get(string name)
        {
            return new NamespaceID(VanillaMod.spaceName, name);
        }
    }
}
