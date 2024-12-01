using MVZ2Logic.Level;
using PVZEngine;
using PVZEngine.Level;
using Tools;
using UnityEngine;

namespace MVZ2.Vanilla.Level
{
    public static class VanillaLevelProps
    {
        public const string ENEMY_POOL = "enemyPool";
        public const string LAST_ENEMY_POSITION = "lastEnemyPosition";
        public const string STARSHARD_COUNT = "starshardCount";
        public const string STARSHARD_SLOT_COUNT = "starshardSlotCount";
        public const string MUSIC_ID = "musicId";
        public const string PICKAXE_DISABLED = "pickaxeDisabled";
        public const string PICKAXE_DISABLE_MESSAGE = "pickaxeDisableMessage";
        public static IEnemySpawnEntry[] GetEnemyPool(this LevelEngine game)
        {
            return game.GetProperty<IEnemySpawnEntry[]>(ENEMY_POOL);
        }
        public static void SetEnemyPool(this LevelEngine game, IEnemySpawnEntry[] value)
        {
            game.SetProperty(ENEMY_POOL, value);
        }
        public static int GetStarshardSlotCount(this LevelEngine game)
        {
            return game.GetProperty<int>(STARSHARD_SLOT_COUNT);
        }
        public static void SetStarshardSlotCount(this LevelEngine game, int value)
        {
            game.SetProperty(STARSHARD_SLOT_COUNT, value);
        }
        public static bool IsPickaxeDisabled(this LevelEngine level)
        {
            return level.GetProperty<bool>(PICKAXE_DISABLED);
        }
        public static string GetPickaxeDisableMessage(this LevelEngine level)
        {
            return level.GetProperty<string>(PICKAXE_DISABLE_MESSAGE);
        }
        public static Vector3 GetLastEnemyPosition(this LevelEngine game)
        {
            return game.GetProperty<Vector3>(LAST_ENEMY_POSITION);
        }
        public static NamespaceID GetMusicID(this LevelEngine game)
        {
            return game.GetProperty<NamespaceID>(MUSIC_ID);
        }
        public static void SetLastEnemyPosition(this LevelEngine game, Vector3 value)
        {
            game.SetProperty(LAST_ENEMY_POSITION, value);
        }
        public static int GetStarshardCount(this LevelEngine game)
        {
            return game.GetProperty<int>(STARSHARD_COUNT);
        }
        public static void SetStarshardCount(this LevelEngine game, int value)
        {
            game.SetProperty(STARSHARD_COUNT, value);
        }
        public static void AddStarshardCount(this LevelEngine game, int value)
        {
            game.SetStarshardCount(game.GetStarshardCount() + value);
        }

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
            level.SetStarshardChance(level.GetStarshardChance() + value);
        }
    }
}
