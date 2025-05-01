using MVZ2.Vanilla;
using PVZEngine;

namespace MVZ2.GameContent.Grids
{
    public static class VanillaGridNames
    {
        public const string grass = "grass";
        public const string water = "water";
        public const string wood = "wood";
        public const string stone = "stone";
    }
    public static class VanillaGridID
    {
        public static readonly NamespaceID grass = Get(VanillaGridNames.grass);
        public static readonly NamespaceID water = Get(VanillaGridNames.water);
        public static readonly NamespaceID wood = Get(VanillaGridNames.wood);
        public static readonly NamespaceID stone = Get(VanillaGridNames.stone);
        private static NamespaceID Get(string name)
        {
            return new NamespaceID(VanillaMod.spaceName, name);
        }
    }
}
