using MVZ2Logic.Level;
using PVZEngine;
using PVZEngine.Definitions;
using PVZEngine.Level;
using UnityEngine;

namespace MVZ2.Vanilla.Level
{
    [PropertyRegistryRegion(PropertyRegions.level)]
    public static class VanillaLevelProps
    {
        private static PropertyMeta<T> Get<T>(string name)
        {
            return new PropertyMeta<T>(name);
        }

        #region 假定存在敌人
        public static readonly PropertyMeta<bool> ASSUME_HAS_ENEMIES = Get<bool>("assume_has_enemies");
        public static bool AssumeHasEnemies(this LevelEngine game)
        {
            return game.GetProperty<bool>(ASSUME_HAS_ENEMIES);
        }
        #endregion

        #region 出怪池

        public static readonly PropertyMeta<NamespaceID[]> ENEMY_POOL = Get<NamespaceID[]>("enemyPool");
        public static NamespaceID[] GetEnemyPool(this LevelEngine game)
        {
            return game.GetProperty<NamespaceID[]>(ENEMY_POOL);
        }
        public static void SetEnemyPool(this LevelEngine game, NamespaceID[] value)
        {
            game.SetProperty(ENEMY_POOL, value);
        }
        #endregion

        #region 传送带
        public static readonly PropertyMeta<IConveyorPoolEntry[]> CONVEYOR_POOL = Get<IConveyorPoolEntry[]>("conveyorPool");
        public static readonly PropertyMeta<float> CONVEY_SPEED = Get<float>("conveySpeed");
        public static IConveyorPoolEntry[] GetConveyorPool(this LevelEngine game)
        {
            return game.GetProperty<IConveyorPoolEntry[]>(CONVEYOR_POOL);
        }
        public static float GetConveySpeed(this LevelEngine game)
        {
            return game.GetProperty<float>(CONVEY_SPEED);
        }
        #endregion

        #region 星之碎片
        public static readonly PropertyMeta<int> STARSHARD_COUNT = Get<int>("starshardCount");
        public static readonly PropertyMeta<int> STARSHARD_SLOT_COUNT = Get<int>("starshardSlotCount");
        public static readonly PropertyMeta<NamespaceID> STARSHARD_DISABLE_ID = Get<NamespaceID>("starshardDisableID");
        public static readonly PropertyMeta<bool> STARSHARD_DISABLE_ICON = Get<bool>("starshardDisableIcon");
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
            return NamespaceID.IsValid(level.GetStarshardDisableID());
        }
        public static NamespaceID GetStarshardDisableID(this LevelEngine level)
        {
            return level.GetProperty<NamespaceID>(STARSHARD_DISABLE_ID);
        }
        public static bool ShouldShowStarshardDisableIcon(this LevelEngine level)
        {
            return level.GetProperty<bool>(STARSHARD_DISABLE_ICON);
        }
        public static bool CanUseStarshard(this LevelEngine level)
        {
            if (!level.IsStarshardActive())
                return false;
            if (level.IsStarshardDisabled())
                return false;
            if (level.GetStarshardCount() <= 0)
                return false;
            return true;
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
        #endregion

        #region 铁镐
        public static readonly PropertyMeta<NamespaceID> PICKAXE_DISABLE_ID = Get<NamespaceID>("pickaxeDisableID");
        public static readonly PropertyMeta<bool> PICKAXE_DISABLE_ICON = Get<bool>("pickaxeDisableIcon");
        public static readonly PropertyMeta<int> PICKAXE_REMAIN_COUNT = Get<int>("pickaxeRemainCount");
        public static readonly PropertyMeta<int> PICKAXE_COUNT_LIMIT = Get<int>("pickaxeCountLimit");
        public static bool IsPickaxeDisabled(this LevelEngine level)
        {
            return NamespaceID.IsValid(level.GetPickaxeDisableID());
        }
        public static NamespaceID GetPickaxeDisableID(this LevelEngine level)
        {
            return level.GetProperty<NamespaceID>(PICKAXE_DISABLE_ID);
        }
        public static bool CanUsePickaxe(this LevelEngine level)
        {
            if (!level.IsPickaxeActive())
                return false;
            if (level.IsPickaxeDisabled())
                return false;
            if (level.IsPickaxeCountLimited() && level.GetPickaxeRemainCount() <= 0)
                return false;
            return true;
        }
        public static bool ShouldShowPickaxeDisableIcon(this LevelEngine level)
        {
            return level.GetProperty<bool>(PICKAXE_DISABLE_ICON);
        }
        public static int GetPickaxeRemainCount(this LevelEngine level)
        {
            return level.GetProperty<int>(PICKAXE_REMAIN_COUNT);
        }
        public static void SetPickaxeRemainCount(this LevelEngine level, int value)
        {
            level.SetProperty(PICKAXE_REMAIN_COUNT, value);
        }
        public static void AddPickaxeRemainCount(this LevelEngine level, int value) => level.SetPickaxeRemainCount(level.GetPickaxeRemainCount() + value);
        public static bool IsPickaxeCountLimited(this LevelEngine level)
        {
            return level.GetPickaxeCountLimit() > 0;
        }
        public static int GetPickaxeCountLimit(this LevelEngine level)
        {
            return level.GetProperty<int>(PICKAXE_COUNT_LIMIT);
        }
        public static void SetPickaxeCountLimit(this LevelEngine level, int value)
        {
            level.SetProperty(PICKAXE_COUNT_LIMIT, value);
        }
        public static void SetPickaxeCountLimit(this StageDefinition level, int value)
        {
            level.SetProperty(PICKAXE_COUNT_LIMIT, value);
        }
        #endregion

        #region 触发器
        public static readonly PropertyMeta<NamespaceID> TRIGGER_DISABLE_ID = Get<NamespaceID>("triggerDisableID");
        public static bool CanUseTrigger(this LevelEngine level)
        {
            if (!level.IsTriggerActive())
                return false;
            if (level.IsTriggerDisabled())
                return false;
            return true;
        }
        public static bool IsTriggerDisabled(this LevelEngine level)
        {
            return NamespaceID.IsValid(level.GetTriggerDisableID());
        }
        public static NamespaceID GetTriggerDisableID(this LevelEngine level)
        {
            return level.GetProperty<NamespaceID>(TRIGGER_DISABLE_ID);
        }
        #endregion

        #region 最后敌人位置
        public static readonly PropertyMeta<Vector3> LAST_ENEMY_POSITION = Get<Vector3>("lastEnemyPosition");
        public static Vector3 GetLastEnemyPosition(this LevelEngine game)
        {
            return game.GetProperty<Vector3>(LAST_ENEMY_POSITION);
        }
        public static void SetLastEnemyPosition(this LevelEngine game, Vector3 value)
        {
            game.SetProperty(LAST_ENEMY_POSITION, value);
        }
        #endregion

        #region 音乐ID
        public static readonly PropertyMeta<NamespaceID> MUSIC_ID = Get<NamespaceID>("musicId");
        public static NamespaceID GetMusicID(this LevelEngine game)
        {
            return game.GetProperty<NamespaceID>(MUSIC_ID);
        }
        public static void SetMusicID(this LevelEngine game, NamespaceID value)
        {
            game.SetProperty(MUSIC_ID, value);
        }
        #endregion

        #region 让手持物品保持在屏幕内
        public static readonly PropertyMeta<bool> KEEP_HELD_ITEM_IN_SCREEN = Get<bool>("keepHeldItemInScreen");
        public static bool KeepHeldItemInScreen(this LevelEngine level)
        {
            return level.GetProperty<bool>(KEEP_HELD_ITEM_IN_SCREEN);
        }
        #endregion

        #region 所有怪物被清理
        public static readonly PropertyMeta<bool> ALL_ENEMIES_CLEARED = Get<bool>("allEnemiesCleared");
        public static bool IsAllEnemiesCleared(this LevelEngine level)
        {
            return level.GetProperty<bool>(ALL_ENEMIES_CLEARED);
        }
        public static void SetAllEnemiesCleared(this LevelEngine level, bool value)
        {
            level.SetProperty(ALL_ENEMIES_CLEARED, value);
        }
        #endregion

        #region 雕像数量

        public static readonly PropertyMeta<int> STATUE_COUNT = Get<int>("statueCount");
        public static int GetStatueCount(this LevelEngine level)
        {
            return level.GetProperty<int>(STATUE_COUNT);
        }
        #endregion

        #region 刷怪笼数量
        public static readonly PropertyMeta<int> SPAWNER_COUNT = Get<int>("spawnerCount");
        public static int GetSpawnerCount(this LevelEngine level)
        {
            return level.GetProperty<int>(SPAWNER_COUNT);
        }
        #endregion

        #region 出怪点数
        public static readonly PropertyMeta<float> SPAWN_POINTS_POWER = Get<float>("spawnPointsPower");
        public static readonly PropertyMeta<float> SPAWN_POINTS_MUTLIPLIER = Get<float>("spawnPointsMultiplier");
        public static readonly PropertyMeta<float> SPAWN_POINTS_ADDITION = Get<float>("spawnPointsAddition");
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

        #region 忽略一大波事件
        public static readonly PropertyMeta<bool> IGNORE_HUGE_WAVE_EVENT = Get<bool>("ignoreHugeWaveEvent");
        public static bool IgnoreHugeWaveEvent(this LevelEngine level)
        {
            return level.GetProperty<bool>(IGNORE_HUGE_WAVE_EVENT);
        }
        public static void SetIgnoreHugeWaveEvent(this LevelEngine level, bool value)
        {
            level.SetProperty(IGNORE_HUGE_WAVE_EVENT, value);
        }
        #endregion

        #region 无推车
        public static readonly PropertyMeta<bool> NO_CARTS = Get<bool>("noCarts");
        public static bool HasNoCarts(this LevelEngine level)
        {
            return level.GetProperty<bool>(NO_CARTS);
        }
        public static void SetNoCarts(this LevelEngine level, bool value)
        {
            level.SetProperty(NO_CARTS, value);
        }
        #endregion

        #region 上帝模式
        public static readonly PropertyMeta<bool> GODMODE = Get<bool>("godmode");
        public static bool IsGodMode(this LevelEngine level)
        {
            return level.GetProperty<bool>(GODMODE);
        }
        public static void SetGodMode(this LevelEngine level, bool value)
        {
            level.SetProperty(GODMODE, value);
        }
        #endregion
    }
}
