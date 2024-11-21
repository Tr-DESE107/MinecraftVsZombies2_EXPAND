using PVZEngine.Entities;

namespace MVZ2.GameContent.Contraptions
{
    public static class VanillaContraptionProps
    {
        public const string PLACE_ON_WATER = "placeOnWater";
        public const string PLACE_ON_LILY = "placeOnLily";
        public const string IS_FLOOR = "isFloor";
        public const string FRAGMENT_ID = "fragmentID";
        public static bool IsFloor(this Entity contraption)
        {
            return contraption.GetProperty<bool>(IS_FLOOR);
        }
    }
}
