using System;
using System.Collections.Generic;
using System.Linq;
using PVZEngine;

namespace MVZ2Logic.Saves
{
    public class EndlessRecord
    {
        public EndlessRecord(string id)
        {
            ID = id;
        }
        public int GetMaxFlags()
        {
            return maxFlags;
        }
        public void SetMaxFlags(int flags)
        {
            maxFlags = flags;
        }
        public SerializableEndlessRecord ToSerializable()
        {
            return new SerializableEndlessRecord()
            {
                id = ID,
                maxFlags = maxFlags,
            };
        }
        public static EndlessRecord FromSerializable(SerializableEndlessRecord serializable)
        {
            return new EndlessRecord(serializable.id)
            {
                maxFlags = serializable.maxFlags
            };
        }
        public string ID { get; private set; }
        private int maxFlags;
    }
    [Serializable]
    public class SerializableEndlessRecord
    {
        public string id;
        public int maxFlags;
    }
}
