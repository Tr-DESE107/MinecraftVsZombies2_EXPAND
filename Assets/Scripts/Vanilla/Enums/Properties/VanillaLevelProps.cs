using PVZEngine.Level;
using Tools;

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

        public const string STARSHARD_RNG = "StarshardRNG";
        public const string STARSHARD_CHANCE = "StarshardChance";
        public static RandomGenerator GetStarshardRNG(this LevelEngine level)
        {
            return level.GetProperty<RandomGenerator>(STARSHARD_RNG);
        }
        public static void SetStarshardRNG(this LevelEngine level, RandomGenerator value)
        {
            level.SetProperty(STARSHARD_RNG, value);
        }
        public static int GetStarshardChance(this LevelEngine level)
        {
            return level.GetProperty<int>(STARSHARD_CHANCE);
        }
        public static void SetStarshardChance(this LevelEngine level, int value)
        {
            level.SetProperty(STARSHARD_CHANCE, value);
        }
        public static void AddStarshardChance(this LevelEngine level, int value)
        {
            SetStarshardChance(level, GetStarshardChance(level) + value);
        }
    }
}
