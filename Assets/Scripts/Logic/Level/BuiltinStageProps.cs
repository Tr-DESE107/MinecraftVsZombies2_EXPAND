using PVZEngine;
using PVZEngine.Level;

namespace MVZ2Logic.Level
{
    public static class BuiltinStageProps
    {
        public const string LEVEL_NAME = "levelName";
        public const string DAY_NUMBER = "dayNumber";

        public const string WAVE_MAX_TIME = "waveMaxTime";
        public const string WAVE_ADVANCE_TIME = "waveAdvanceTime";
        public const string WAVE_ADVANCE_HEALTH_PERCENT = "waveAdvanceHealthPercent";
        public const string NO_PRODUCTION = "noProduction";
        public const string AUTO_COLLECT = "autoCollect";

        public const string START_TALK = "startTalk";
        public const string END_TALK = "endTalk";
        public const string MAP_TALK = "mapTalk";

        public const string CLEAR_PICKUP_MODEL = "clearPickupModel";
        public const string CLEAR_PICKUP_BLUEPRINT = "clearPickupBlueprint";
        public const string END_NOTE_ID = "endNoteId";

        public const string START_TRANSITION = "startTransition";
        public const string START_CAMERA_POSITION = "startCameraPosition";

        public static NamespaceID GetClearPickupModel(this LevelEngine level)
        {
            return level.GetProperty<NamespaceID>(CLEAR_PICKUP_MODEL);
        }
        public static NamespaceID GetClearPickupBlueprint(this LevelEngine level)
        {
            return level.GetProperty<NamespaceID>(CLEAR_PICKUP_BLUEPRINT);
        }
    }
}
