using MVZ2Logic;
using PVZEngine;
using PVZEngine.Definitions;
using PVZEngine.Level;
using UnityEngine;

namespace MVZ2.Vanilla.Level
{
    public static class VanillaAreaProps
    {
        public const string MODEL_ID = "modelID";
        public static NamespaceID GetModelID(this LevelEngine game)
        {
            return game.GetProperty<NamespaceID>(MODEL_ID);
        }
        public static NamespaceID GetModelID(this AreaDefinition definition)
        {
            return definition.GetProperty<NamespaceID>(MODEL_ID);
        }
        public const string STARSHARD_ICON = "starshardIcon";
        public static SpriteReference GetStarshardIcon(this LevelEngine game)
        {
            return game.GetProperty<SpriteReference>(STARSHARD_ICON);
        }
        public const string WATER_COLOR = "waterColor";
        public const string WATER_COLOR_CENSORED = "waterColorCensored";
        public static Color GetWaterColorNormal(this LevelEngine game)
        {
            return game.GetProperty<Color>(WATER_COLOR);
        }
        public static Color GetWaterColorCensored(this LevelEngine game)
        {
            return game.GetProperty<Color>(WATER_COLOR_CENSORED);
        }
        public static Color GetWaterColor(this LevelEngine game)
        {
            return Global.HasBloodAndGore() ? game.GetWaterColorNormal() : game.GetWaterColorCensored();
        }

        public const string DOOR_Z = "doorZ";
        public const string NIGHT_VALUE = "nightValue";
        public const string DARKNESS_VALUE = "darknessValue";

        public static float GetDoorZ(this LevelEngine game)
        {
            return game.GetProperty<float>(DOOR_Z);
        }
        public static float GetNightValue(this LevelEngine level)
        {
            return level.GetProperty<float>(NIGHT_VALUE);
        }
        public static float GetDarknessValue(this LevelEngine level)
        {
            return level.GetProperty<float>(DARKNESS_VALUE);
        }
    }
}
