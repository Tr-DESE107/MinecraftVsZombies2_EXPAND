using MVZ2Logic.Artifacts;
using MVZ2Logic.Callbacks;
using MVZ2Logic.Triggers;
using PVZEngine;
using PVZEngine.Entities;
using PVZEngine.Level;
using PVZEngine.Triggers;
using UnityEngine;

namespace MVZ2Logic.Level
{
    public static class LogicLevelProps
    {
        public const string SCREEN_COVER = "screenCover";
        public static Color GetScreenCover(this LevelEngine level)
        {
            return level.GetProperty<Color>(SCREEN_COVER);
        }
        public static void SetScreenCover(this LevelEngine level, Color value)
        {
            level.SetProperty(SCREEN_COVER, value);
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
