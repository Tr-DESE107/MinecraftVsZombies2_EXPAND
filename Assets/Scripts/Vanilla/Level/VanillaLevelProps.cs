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
        public const string CONVEYOR_POOL = "conveyorPool";
        public const string CONVEY_SPEED = "conveySpeed";
        public const string LAST_ENEMY_POSITION = "lastEnemyPosition";
        public const string STARSHARD_COUNT = "starshardCount";
        public const string STARSHARD_SLOT_COUNT = "starshardSlotCount";
        public const string MUSIC_ID = "musicId";
        public const string PICKAXE_DISABLED = "pickaxeDisabled";
        public const string PICKAXE_DISABLE_MESSAGE = "pickaxeDisableMessage";
        public const string KEEP_HELD_ITEM_IN_SCREEN = "keepHeldItemInScreen";
        public const string TRIGGER_DISABLED = "triggerDisabled";
        public const string TRIGGER_DISABLE_MESSAGE = "triggerDisableMessage";
        public static NamespaceID[] GetEnemyPool(this LevelEngine game)
        {
            return game.GetProperty<NamespaceID[]>(ENEMY_POOL);
        }
        public static void SetEnemyPool(this LevelEngine game, NamespaceID[] value)
        {
            game.SetProperty(ENEMY_POOL, value);
        }
        public static IConveyorPoolEntry[] GetConveyorPool(this LevelEngine game)
        {
            return game.GetProperty<IConveyorPoolEntry[]>(CONVEYOR_POOL);
        }
        public static float GetConveySpeed(this LevelEngine game)
        {
            return game.GetProperty<float>(CONVEY_SPEED);
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
        public static bool IsTriggerDisabled(this LevelEngine level)
        {
            return level.GetProperty<bool>(TRIGGER_DISABLED);
        }
        public static string GetTriggerDisableMessage(this LevelEngine level)
        {
            return level.GetProperty<string>(TRIGGER_DISABLE_MESSAGE);
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
        public static bool KeepHeldItemInScreen(this LevelEngine level)
        {
            return level.GetProperty<bool>(KEEP_HELD_ITEM_IN_SCREEN);
        }

        public const string FIRST_GEM = "firstGem";
        public const string ALL_ENEMIES_CLEARED = "allEnemiesCleared";
        public const string STATUE_COUNT = "statueCount";
        public static int GetStatueCount(this LevelEngine level)
        {
            return level.GetProperty<int>(STATUE_COUNT);
        }
        public static bool IsAllEnemiesCleared(this LevelEngine level)
        {
            return level.GetProperty<bool>(VanillaLevelProps.ALL_ENEMIES_CLEARED);
        }
        public static void SetAllEnemiesCleared(this LevelEngine level, bool value)
        {
            level.SetProperty(VanillaLevelProps.ALL_ENEMIES_CLEARED, value);
        }
        public const string SPAWN_POINTS = "SpawnPoints";
        public static float GetSpawnPoints(this LevelEngine level)
        {
            return level.GetProperty<float>(SPAWN_POINTS);
        }
        public static void SetSpawnPoints(this LevelEngine level, float value)
        {
            level.SetProperty(SPAWN_POINTS, value);
        }
        public const string IGNORE_HUGE_WAVE_EVENT = "ignoreHugeWaveEvent";
        public static bool IgnoreHugeWaveEvent(this LevelEngine level)
        {
            return level.GetProperty<bool>(IGNORE_HUGE_WAVE_EVENT);
        }
        public static void SetIgnoreHugeWaveEvent(this LevelEngine level, bool value)
        {
            level.SetProperty(IGNORE_HUGE_WAVE_EVENT, value);
        }
        
    }
}
