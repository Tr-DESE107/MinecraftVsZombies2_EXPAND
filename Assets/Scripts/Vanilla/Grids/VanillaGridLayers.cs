using PVZEngine;

namespace MVZ2.Vanilla.Grids
{
    public static class VanillaGridLayers
    {
        public static readonly NamespaceID main = Get("main");
        public static readonly NamespaceID carrier = Get("main");
        public static readonly NamespaceID protector = Get("main");
        private static NamespaceID Get(string name)
        {
            return new NamespaceID(VanillaMod.spaceName, name);
        }
    }
}
