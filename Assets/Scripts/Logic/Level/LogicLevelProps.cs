using MVZ2Logic.Artifacts;
using MVZ2Logic.Callbacks;
using MVZ2Logic.Triggers;
using PVZEngine;
using PVZEngine.Entities;
using PVZEngine.Level;
using PVZEngine.Triggers;

namespace MVZ2Logic.Level
{
    public static class LogicLevelProps
    {
        public const string BLACKSCREEN = "blackscreen";
        public static float GetBlackscreen(this LevelEngine level)
        {
            return level.GetProperty<float>(BLACKSCREEN);
        }
        public static void SetBlackscreen(this LevelEngine level, float value)
        {
            level.SetProperty(BLACKSCREEN, value);
        }
        public const string PAUSE_DISABLED = "pause_disabled";
        public static bool IsPauseDisabled(this LevelEngine level)
        {
            return level.GetProperty<bool>(PAUSE_DISABLED);
        }
        public static void SetPauseDisabled(this LevelEngine level, bool value)
        {
            level.SetProperty(PAUSE_DISABLED, value);
        }
    }
}
