using PVZEngine;
using PVZEngine.Level;
using UnityEngine;

namespace MVZ2Logic.Level
{
    [PropertyRegistryRegion(PropertyRegions.level)]
    public static class LogicLevelProps
    {
        public static readonly PropertyMeta SCREEN_COVER = new PropertyMeta("screenCover");
        public static Color GetScreenCover(this LevelEngine level)
        {
            return level.GetProperty<Color>(SCREEN_COVER);
        }
        public static void SetScreenCover(this LevelEngine level, Color value)
        {
            level.SetProperty(SCREEN_COVER, value);
        }
        public static readonly PropertyMeta PAUSE_DISABLED = new PropertyMeta("pause_disabled");
        public static bool IsPauseDisabled(this LevelEngine level)
        {
            return level.GetProperty<bool>(PAUSE_DISABLED);
        }
        public static void SetPauseDisabled(this LevelEngine level, bool value)
        {
            level.SetProperty(PAUSE_DISABLED, value);
        }
        public static readonly PropertyMeta CAMERA_ROTATION = new PropertyMeta("cameraRotation");
        public static float GetCameraRotation(this LevelEngine level)
        {
            return level.GetProperty<float>(CAMERA_ROTATION);
        }
        public static void SetCameraRotation(this LevelEngine level, float value)
        {
            level.SetProperty(CAMERA_ROTATION, value);
        }
    }
}
