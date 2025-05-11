using MVZ2.Vanilla;
using PVZEngine;
using PVZEngine.Placements;

namespace MVZ2.GameContent.Placements
{
    public static class VanillaPlaceMethods
    {
        public static readonly PlaceMethod entity = new EntityPlaceMethod();
        public static readonly PlaceMethod upgrade = new UpgradePlaceMethod();
        public static readonly PlaceMethod drivenser = new DrivenserPlaceMethod();
        public static readonly PlaceMethod firstAid = new FirstAidPlaceMethod();
    }
}
