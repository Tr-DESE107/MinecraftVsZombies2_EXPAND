using PVZEngine.Level;

namespace MVZ2.GameContent
{
    public static class VanillaLevelProps
    {
        public const string FIRST_GEM = "firstGem";
        public const string ALL_ENEMIES_CLEARED = "allEnemiesCleared";
        public const string STATUE_COUNT = "statueCount";
        public static int GetStatueCount(this LevelEngine level)
        {
            return level.GetProperty<int>(STATUE_COUNT);
        }
    }
}
