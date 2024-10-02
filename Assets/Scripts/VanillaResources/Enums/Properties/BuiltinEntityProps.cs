using PVZEngine.Definitions;
using PVZEngine.Level;

namespace MVZ2.GameContent
{
    public static class BuiltinEntityProps
    {
        public const string DEATH_SOUND = "deathSound";
        public const string PLACE_SOUND = "placeSound";
        public const string MAX_TIMEOUT = "maxTimeout";

        public const string CHANGING_LANE = "isChangingLane";
        public const string CHANGE_LANE_SPEED = "changeLaneSpeed";
        public const string CHANGE_LANE_TARGET = "changeLaneTarget";
        public const string CHANGE_LANE_SOURCE = "changeLaneSource";

        public const string SHADOW_HIDDEN = "shadowHidden";
        public const string SHADOW_ALPHA = "shadowAlpha";
        public const string SHADOW_SCALE = "shadowScale";
        public const string SHADOW_OFFSET = "shadowOffset";

        public const string SORTING_LAYER = "sortingLayer";
        public const string SORTING_ORDER = "sortingOrder";

        public const string UPDATE_BEFORE_GAME = "updateBeforeGame";

        public static bool CanUpdateBeforeGameStart(this Entity entity)
        {
            return entity.GetProperty<bool>(UPDATE_BEFORE_GAME);
        }
    }
}
