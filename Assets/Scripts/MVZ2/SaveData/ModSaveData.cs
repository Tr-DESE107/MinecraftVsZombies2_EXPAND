using System;
using System.Collections.Generic;
using System.Linq;
using PVZEngine;

namespace MVZ2.Save
{
    public abstract class ModSaveData
    {
        public ModSaveData(string spaceName)
        {
            Namespace = spaceName;
        }
        public void Unlock(string unlockID)
        {
            unlocks.Add(unlockID);
        }
        public bool IsUnlocked(string unlockID)
        {
            return unlocks.Contains(unlockID);
        }
        public void AddLevelDifficultyRecord(string levelID, NamespaceID difficulty)
        {
            var record = GetLevelDifficultyRecord(levelID);
            if (record == null)
            {
                record = CreateLevelDifficultyRecord(levelID);
            }
            record.AddRecord(difficulty);
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
        public SerializableModSaveData ToSerializable()
        {
            var serializable = CreateSerializable();
            serializable.spaceName = Namespace;
            serializable.stats = stats.ToSerializable();
            serializable.levelDifficultyRecords = levelDifficultyRecords.Select(r => r.ToSerializable()).ToArray();
            serializable.unlocks = unlocks.ToArray();
            return serializable;
        }
        protected abstract SerializableModSaveData CreateSerializable();
        protected void LoadFromSerializable(SerializableModSaveData serializable)
        {
            stats = UserStats.FromSerializable(serializable.stats);
            levelDifficultyRecords = serializable.levelDifficultyRecords.Select(r => LevelDifficultyRecord.FromSerializable(r)).ToList();
            unlocks = serializable.unlocks.ToHashSet();
        }
        public string Namespace { get; private set; }
        protected UserStats stats = new UserStats();
        protected List<LevelDifficultyRecord> levelDifficultyRecords = new List<LevelDifficultyRecord>();
        protected HashSet<string> unlocks = new HashSet<string>();
    }
    public abstract class SerializableModSaveData
    {
        public string spaceName;
        public SerializableUserStats stats;
        public SerializableLevelDifficultyRecord[] levelDifficultyRecords;
        public string[] unlocks;
    }
}
