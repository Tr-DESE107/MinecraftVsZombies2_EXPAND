using PVZEngine;
using PVZEngine.Placements;

namespace MVZ2.GameContent.Placements
{
    [PropertyRegistryRegion(PropertyRegions.placeParams)]
    public static class VanillaPlaceProps
    {
        public static readonly PropertyMeta COMMAND_BLOCK = new PropertyMeta("commandBlock");
        public static bool IsCommandBlock(this PlaceParams param) => param.GetProperty<bool>(COMMAND_BLOCK);
        public static void SetCommandBlock(this PlaceParams param, bool value) => param.SetProperty(COMMAND_BLOCK, value);
    }
}
