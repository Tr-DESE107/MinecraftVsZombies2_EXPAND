using System;
using System.Collections.Generic;
using System.Linq;
using PVZEngine;

namespace MVZ2Logic.Saves
{
    public abstract class ModSaveData
    {
        public ModSaveData(string spaceName)
        {
            Namespace = spaceName;
        }
        #region 解锁
        public void Unlock(string unlockID)
        {
            unlocks.Add(unlockID);
        }
        public bool IsUnlocked(string unlockID)
        {
            return unlocks.Contains(unlockID);
        }
        #endregion

        #region 关卡难度记录
        public void AddLevelDifficultyRecord(string levelID, NamespaceID difficulty)
        {
            var record = GetLevelDifficultyRecord(levelID);
            if (record == null)
            {
                record = CreateLevelDifficultyRecord(levelID);
            }
            record.AddRecord(difficulty);
        }
        public bool RemoveLevelDifficultyRecord(string levelID, NamespaceID difficulty)
        {
            var record = GetLevelDifficultyRecord(levelID);
            if (record == null)
            {
                return false;
            }
            return record.RemoveRecord(difficulty);
        }
        public bool HasLevelDifficultyRecord(string levelID, NamespaceID difficulty)
        {
            var record = GetLevelDifficultyRecord(levelID);
            if (record == null)
                return false;
            return record.HasRecord(difficulty);
        }
        public NamespaceID[] GetLevelDifficultyRecords(string levelID)
        {
            var record = GetLevelDifficultyRecord(levelID);
            if (record == null)
                return Array.Empty<NamespaceID>();
            return record.GetAllRecords();
        }
        private LevelDifficultyRecord CreateLevelDifficultyRecord(string levelID)
        {
            var record = new LevelDifficultyRecord(levelID);
            levelDifficultyRecords.Add(record);
            return record;
        }
        private LevelDifficultyRecord GetLevelDifficultyRecord(string levelID)
        {
            return levelDifficultyRecords.FirstOrDefault(r => r.ID == levelID); ;
        }
        #endregion

        #region 地图预设
        public void SetMapPresetID(string mapId, NamespaceID preset)
        {
            var config = GetMapPresetConfig(mapId);
            if (config == null)
            {
                config = CreateMapPresetConfig(mapId);
            }
            config.Preset = preset;
        }
        public NamespaceID GetMapPresetID(string mapId)
        {
            var config = GetMapPresetConfig(mapId);
            if (config == null)
                return null;
            return config.Preset;
        }
        private MapPresetConfig CreateMapPresetConfig(string mapId)
        {
            var config = new MapPresetConfig(mapId);
            mapPresetConfigs.Add(config);
            return config;
        }
        private MapPresetConfig GetMapPresetConfig(string mapId)
        {
            return mapPresetConfigs.FirstOrDefault(r => r.ID == mapId);
        }
        #endregion

        #region 无尽
        public void SetCurrentEndlessFlag(string stageID, int value)
        {
            var record = endlessRecords.FirstOrDefault(e => e.ID == stageID);
            if (record == null)
            {
                record = new EndlessRecord(stageID);
                endlessRecords.Add(record);
            }
            record.SetMaxFlags(value);
        }
        public int GetCurrentEndlessFlag(string stageID)
        {
            var record = endlessRecords.FirstOrDefault(e => e.ID == stageID);
            if (record == null)
            {
                return 0;
            }
            return record.GetMaxFlags();
        }
        #endregion

        #region 统计
        public void SetStat(string category, NamespaceID entry, long value)
        {
            stats.SetStatValue(category, entry, value);
        }
        public long GetStat(string category, NamespaceID entry)
        {
            return stats.GetStatValue(category, entry);
        }
        public void AddStat(string category, NamespaceID entry, long value)
        {
            SetStat(category, entry, GetStat(category, entry) + value);
        }
        public UserStats GetAllStats() => stats;
        #endregion

        public SerializableModSaveData ToSerializable()
        {
            var serializable = CreateSerializable();
            serializable.spaceName = Namespace;
            serializable.stats = stats.ToSerializable();
            serializable.endlessRecords = endlessRecords.Select(r => r.ToSerializable()).ToArray();
            serializable.levelDifficultyRecords = levelDifficultyRecords.Select(r => r.ToSerializable()).ToArray();
            serializable.mapPresetConfigs = mapPresetConfigs.Select(i => i.ToSerializable()).ToArray();
            serializable.unlocks = unlocks.ToArray();
            return serializable;
        }
        protected abstract SerializableModSaveData CreateSerializable();
        protected void LoadFromSerializable(SerializableModSaveData serializable)
        {
            stats = UserStats.FromSerializable(serializable.stats);
            endlessRecords = serializable.endlessRecords?.Select(i => EndlessRecord.FromSerializable(i))?.ToList() ?? new List<EndlessRecord>();
            levelDifficultyRecords = serializable.levelDifficultyRecords.Select(r => LevelDifficultyRecord.FromSerializable(r)).ToList();
            mapPresetConfigs = serializable.mapPresetConfigs?.Select(i => MapPresetConfig.FromSerializable(i))?.ToList() ?? new List<MapPresetConfig>();
            unlocks = serializable.unlocks.ToHashSet();
        }
        public string Namespace { get; private set; }
        protected UserStats stats = new UserStats();
        protected List<LevelDifficultyRecord> levelDifficultyRecords = new List<LevelDifficultyRecord>();
        protected List<EndlessRecord> endlessRecords = new List<EndlessRecord>();
        protected List<MapPresetConfig> mapPresetConfigs = new List<MapPresetConfig>();
        protected HashSet<string> unlocks = new HashSet<string>();
    }
    public abstract class SerializableModSaveData
    {
        public int version;
        public string spaceName;
        public SerializableUserStats stats;
        public SerializableEndlessRecord[] endlessRecords;
        public SerializableLevelDifficultyRecord[] levelDifficultyRecords;
        public SerializableMapPresetConfig[] mapPresetConfigs;
        public string[] unlocks;
    }
}
