using PVZEngine;

namespace MVZ2.Vanilla.Grids
{
    public static class VanillaGridLayers
    {
        public static readonly NamespaceID main = Get("main");
        public static readonly NamespaceID carrier = Get("carrier");
        public static readonly NamespaceID protector = Get("protector");
        public static readonly NamespaceID tool = Get("tool");

        public static NamespaceID[] protectedLayers = new NamespaceID[]
        {
            tool,
            main,
            carrier
        };
        public static NamespaceID[] sacrificeLayers = new NamespaceID[]
        {
            protector,
            tool,
            main,
            carrier
        };
        public static NamespaceID[] dreamSilkLayers = new NamespaceID[]
        {
            protector,
            tool,
            main,
            carrier
        };
        public static NamespaceID[] devourerLayers = new NamespaceID[]
        {
            main,
            protector,
            carrier
        };
        public static NamespaceID[] ufoLayers = new NamespaceID[]
        {
            main,
            protector,
            carrier
        };


        private static NamespaceID Get(string name)
        {
            return new NamespaceID(VanillaMod.spaceName, name);
        }
    }
}
