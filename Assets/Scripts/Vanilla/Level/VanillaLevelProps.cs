using MVZ2Logic.Level;
using PVZEngine;
using PVZEngine.Definitions;
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
        public const string MUSIC_ID = "musicId";
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
        #region 星之碎片
        public const string STARSHARD_COUNT = "starshardCount";
        public const string STARSHARD_SLOT_COUNT = "starshardSlotCount";
        public const string STARSHARD_DISABLED = "starshardDisabled";
        public const string STARSHARD_DISABLE_MESSAGE = "starshardDisableMessage";
        public const string STARSHARD_DISABLE_ICON = "starshardDisableIcon";
        public static int GetStarshardSlotCount(this LevelEngine game)
        {
            return game.GetProperty<int>(STARSHARD_SLOT_COUNT);
        }
        public static void SetStarshardSlotCount(this LevelEngine game, int value)
        {
            game.SetProperty(STARSHARD_SLOT_COUNT, value);
        }
        public static bool IsStarshardDisabled(this LevelEngine level)
        {
            return level.GetProperty<bool>(STARSHARD_DISABLED);
        }
        public static bool ShouldShowStarshardDisableIcon(this LevelEngine level)
        {
            return level.GetProperty<bool>(STARSHARD_DISABLE_ICON);
        }
        #endregion

        #region 铁镐
        public const string PICKAXE_DISABLED = "pickaxeDisabled";
        public const string PICKAXE_DISABLE_MESSAGE = "pickaxeDisableMessage";
        public const string PICKAXE_DISABLE_ICON = "pickaxeDisableIcon";
        public static bool IsPickaxeDisabled(this LevelEngine level)
        {
            return level.GetProperty<bool>(PICKAXE_DISABLED);
        }
        public static string GetPickaxeDisableMessage(this LevelEngine level)
        {
            return level.GetProperty<string>(PICKAXE_DISABLE_MESSAGE);
        }
        public static bool ShouldShowPickaxeDisableIcon(this LevelEngine level)
        {
            return level.GetProperty<bool>(PICKAXE_DISABLE_ICON);
        }
        #endregion
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
        public static void SetMusicID(this LevelEngine game, NamespaceID value)
        {
            game.SetProperty(MUSIC_ID, value);
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
        #region 出怪
        public const string SPAWN_POINTS_POWER = "spawnPointsPower";
        public const string SPAWN_POINTS_MUTLIPLIER = "spawnPointsMultiplier";
        public const string SPAWN_POINTS_ADDITION = "spawnPointsAddition";
        public static float GetSpawnPointPower(this LevelEngine level)
        {
            return level.GetProperty<float>(SPAWN_POINTS_POWER);
        }
        public static void SetSpawnPointPower(this StageDefinition stageDef, float value)
        {
            stageDef.SetProperty(SPAWN_POINTS_POWER, value);
        }
        public static float GetSpawnPointMultiplier(this LevelEngine level)
        {
            return level.GetProperty<float>(SPAWN_POINTS_MUTLIPLIER);
        }
        public static void SetSpawnPointMultiplier(this StageDefinition stageDef, float value)
        {
            stageDef.SetProperty(SPAWN_POINTS_MUTLIPLIER, value);
        }
        public static float GetSpawnPointAddition(this LevelEngine level)
        {
            return level.GetProperty<float>(SPAWN_POINTS_ADDITION);
        }
        public static void SetSpawnPointAddition(this StageDefinition stageDef, float value)
        {
            stageDef.SetProperty(SPAWN_POINTS_ADDITION, value);
        }
        #endregion
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
