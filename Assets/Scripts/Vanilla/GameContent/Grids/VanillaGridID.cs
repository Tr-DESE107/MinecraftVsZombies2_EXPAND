using MVZ2.Vanilla;
using PVZEngine;

namespace MVZ2.GameContent.Grids
{
    public static class VanillaGridNames
    {
        public const string grass = "grass";
        public const string water = "water";
    }
    public static class VanillaGridID
    {
        public static readonly NamespaceID grass = Get(VanillaGridNames.grass);
        public static readonly NamespaceID water = Get(VanillaGridNames.water);
        private static NamespaceID Get(string name)
        {
            return new NamespaceID(VanillaMod.spaceName, name);
        }
    }
}
