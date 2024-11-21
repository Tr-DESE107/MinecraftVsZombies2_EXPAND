using System;
using System.Collections.Generic;
using System.Linq;

namespace MVZ2Logic.Saves
{
    public class UserStats
    {
        public long GetStatValue(string id)
        {
            var entry = GetEntry(id);
            if (entry == null)
                return 0;
            return entry.Value;
        }
        public void SetStatValue(string id, long value)
        {
            var entry = GetEntry(id);
            if (entry == null)
                entry = CreateEntry(id);
            entry.Value = value;
        }
        public bool HasStat(string id)
        {
            return entries.Exists(e => e.ID == id);
        }
        public SerializableUserStats ToSerializable()
        {
            return new SerializableUserStats()
            {
                entries = entries.Select(e => e.ToSerializable()).ToArray()
            };
        }
        public static UserStats FromSerializable(SerializableUserStats serializable)
        {
            var stats = new UserStats();
            stats.entries.AddRange(serializable.entries.Select(e => UserStatEntry.FromSerializable(e)));
            return stats;
        }
        private string[] GetAllEntriesID()
        {
            return entries.Select(e => e.ID).ToArray();
        }
        private UserStatEntry GetEntry(string id)
        {
            return entries.FirstOrDefault(e => e.ID == id);
        }
        private UserStatEntry CreateEntry(string id)
        {
            var entry = new UserStatEntry(id);
            entries.Add(entry);
            return entry;
        }
        private List<UserStatEntry> entries = new List<UserStatEntry>();
    }
    [Serializable]
    public class SerializableUserStats
    {
        public SerializableUserStatEntry[] entries;
    }
}
