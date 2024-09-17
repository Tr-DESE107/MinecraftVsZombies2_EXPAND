using System;
using System.Collections.Generic;
using System.Linq;
using PVZEngine;

namespace MVZ2.Save
{
    public class LevelDifficultyRecord
    {
        public LevelDifficultyRecord(string id)
        {
            ID = id;
        }
        public void AddRecord(NamespaceID difficulty)
        {
            difficulties.Add(difficulty);
        }
        public bool HasRecord(NamespaceID difficulty)
        {
            return difficulties.Contains(difficulty);
        }
        public NamespaceID[] GetAllRecords()
        {
            return difficulties.ToArray();
        }
        public SerializableLevelDifficultyRecord ToSerializable()
        {
            return new SerializableLevelDifficultyRecord()
            {
                id = ID,
                difficulties = difficulties.ToArray()
            };
        }
        public static LevelDifficultyRecord FromSerializable(SerializableLevelDifficultyRecord serializable)
        {
            return new LevelDifficultyRecord(serializable.id)
            {
                difficulties = serializable.difficulties.ToHashSet()
            };
        }
        public string ID { get; private set; }
        private HashSet<NamespaceID> difficulties = new HashSet<NamespaceID>();
    }
    [Serializable]
    public class SerializableLevelDifficultyRecord
    {
        public string id;
        public NamespaceID[] difficulties;
    }
}
