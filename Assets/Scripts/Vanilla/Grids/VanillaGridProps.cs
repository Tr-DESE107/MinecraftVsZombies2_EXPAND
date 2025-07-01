using PVZEngine;
using PVZEngine.Grids;

namespace MVZ2.Vanilla.Grids
{
    [PropertyRegistryRegion(PropertyRegions.grid)]
    public static class VanillaGridProps
    {
        private static PropertyMeta<T> Get<T>(string name)
        {
            return new PropertyMeta<T>(name);
        }
        public static readonly PropertyMeta<bool> IS_SLAB = Get<bool>("is_slab");
        public static bool IsSlab(this LawnGrid grid)
        {
            return grid.GetProperty<bool>(IS_SLAB);
        }
        public static readonly PropertyMeta<bool> IS_WATER = Get<bool>("is_water");
        public static bool IsWater(this LawnGrid grid)
        {
            return grid.GetProperty<bool>(IS_WATER);
        }
        public static readonly PropertyMeta<bool> IS_AIR = Get<bool>("is_air");
        public static bool IsAir(this LawnGrid grid)
        {
            return grid.GetProperty<bool>(IS_AIR);
        }

    }
}
