using PVZEngine.Level;

namespace MVZ2.Vanilla.Level
{
    public static class VanillaAreaProps
    {
        public const string DOOR_Z = "doorZ";
        public const string NIGHT_VALUE = "nightValue";

        public static float GetDoorZ(this LevelEngine game)
        {
            return game.GetProperty<float>(DOOR_Z);
        }
        public static float GetNightValue(this LevelEngine level)
        {
            return level.GetProperty<float>(NIGHT_VALUE);
        }
    }
}
