using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson.Serialization.Attributes;
using PVZEngine;

namespace MVZ2Logic.Saves
{
    public class UserStats
    {
        public long GetStatValue(string category, NamespaceID entry)
        {
            var cate = GetCategory(category);
            if (cate == null)
                return 0;
            return cate.GetStatValue(entry);
        }
        public void SetStatValue(string category, NamespaceID entry, long value)
        {
            var cate = GetCategory(category);
            if (cate == null)
                cate = CreateCategory(category);
            cate.SetStatValue(entry, value);
        }
        public bool HasCategory(string name)
        {
            return categories.Exists(e => e.Name == name);
        }
        public UserStatCategory[] GetAllCategories()
        {
            return categories.ToArray();
        }
        public SerializableUserStats ToSerializable()
        {
            return new SerializableUserStats()
            {
                categories = categories.Select(e => e.ToSerializable()).ToArray(),
                playTimeMilliseconds = PlayTimeMilliseconds,
            };
        }
        public static UserStats FromSerializable(SerializableUserStats serializable)
        {
            var stats = new UserStats();
            if (serializable.categories != null)
                stats.categories.AddRange(serializable.categories.Select(e => UserStatCategory.FromSerializable(e)));

            stats.PlayTimeMilliseconds = serializable.playTimeMilliseconds;

            return stats;
        }
        private string[] GetAllCategoryNames()
        {
            return categories.Select(e => e.Name).ToArray();
        }
        private UserStatCategory GetCategory(string name)
        {
            return categories.FirstOrDefault(e => e.Name == name);
        }
        private UserStatCategory CreateCategory(string name)
        {
            var entry = new UserStatCategory(name);
            categories.Add(entry);
            return entry;
        }
        private List<UserStatCategory> categories = new List<UserStatCategory>();
        public long PlayTimeMilliseconds { get; set; }
    }
    [Serializable]
    [BsonIgnoreExtraElements]
    public class SerializableUserStats
    {
        public SerializableUserStatCategory[] categories;
        public long playTimeMilliseconds;
        [Obsolete]
        [BsonIgnoreIfNull]
        public SerializableUserStatEntry[] entries;
    }
}
