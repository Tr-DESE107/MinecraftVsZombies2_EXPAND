using MVZ2Logic.Level;
using PVZEngine;
using PVZEngine.Definitions;
using PVZEngine.Level;
using UnityEngine;

namespace MVZ2.Vanilla.Level
{
    [PropertyRegistryRegion]
    public static class VanillaLevelProps
    {
        private static PropertyMeta Get(string name)
        {
            return new PropertyMeta(name);
        }
        public static readonly PropertyMeta ENEMY_POOL = Get("enemyPool");
        public static readonly PropertyMeta CONVEYOR_POOL = Get("conveyorPool");
        public static readonly PropertyMeta CONVEY_SPEED = Get("conveySpeed");
        public static readonly PropertyMeta LAST_ENEMY_POSITION = Get("lastEnemyPosition");
        public static readonly PropertyMeta MUSIC_ID = Get("musicId");
        public static readonly PropertyMeta KEEP_HELD_ITEM_IN_SCREEN = Get("keepHeldItemInScreen");
        public static readonly PropertyMeta TRIGGER_DISABLED = Get("triggerDisabled");
        public static readonly PropertyMeta TRIGGER_DISABLE_MESSAGE = Get("triggerDisableMessage");
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
        public static readonly PropertyMeta STARSHARD_COUNT = Get("starshardCount");
        public static readonly PropertyMeta STARSHARD_SLOT_COUNT = Get("starshardSlotCount");
        public static readonly PropertyMeta STARSHARD_DISABLED = Get("starshardDisabled");
        public static readonly PropertyMeta STARSHARD_DISABLE_MESSAGE = Get("starshardDisableMessage");
        public static readonly PropertyMeta STARSHARD_DISABLE_ICON = Get("starshardDisableIcon");
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
        public static readonly PropertyMeta PICKAXE_DISABLED = Get("pickaxeDisabled");
        public static readonly PropertyMeta PICKAXE_DISABLE_MESSAGE = Get("pickaxeDisableMessage");
        public static readonly PropertyMeta PICKAXE_DISABLE_ICON = Get("pickaxeDisableIcon");
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
            value = Mathf.Clamp(value, 0, game.GetStarshardSlotCount());
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

        public static readonly PropertyMeta FIRST_GEM = Get("firstGem");
        public static readonly PropertyMeta ALL_ENEMIES_CLEARED = Get("allEnemiesCleared");
        public static readonly PropertyMeta STATUE_COUNT = Get("statueCount");
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
        public static readonly PropertyMeta SPAWN_POINTS_POWER = Get("spawnPointsPower");
        public static readonly PropertyMeta SPAWN_POINTS_MUTLIPLIER = Get("spawnPointsMultiplier");
        public static readonly PropertyMeta SPAWN_POINTS_ADDITION = Get("spawnPointsAddition");
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
        public static readonly PropertyMeta IGNORE_HUGE_WAVE_EVENT = Get("ignoreHugeWaveEvent");
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
