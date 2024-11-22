using PVZEngine.Level;

namespace MVZ2Logic.Level
{
    public static class BuiltinAreaProps
    {
        public const string DOOR_Z = "doorZ";
        public const string NIGHT_VALUE = "nightValue";

        public static float GetNightValue(this LevelEngine level)
        {
            return level.GetProperty<float>(NIGHT_VALUE);
        }
    }
}
