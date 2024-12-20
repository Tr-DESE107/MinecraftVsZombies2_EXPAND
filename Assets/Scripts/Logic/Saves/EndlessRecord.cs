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
            return currentFlag;
        }
        public void SetMaxFlags(int flags)
        {
            currentFlag = flags;
        }
        public SerializableEndlessRecord ToSerializable()
        {
            return new SerializableEndlessRecord()
            {
                id = ID,
                currentFlag = currentFlag,
            };
        }
        public static EndlessRecord FromSerializable(SerializableEndlessRecord serializable)
        {
            return new EndlessRecord(serializable.id)
            {
                currentFlag = serializable.currentFlag
            };
        }
        public string ID { get; private set; }
        private int currentFlag;
    }
    [Serializable]
    public class SerializableEndlessRecord
    {
        public string id;
        public int currentFlag;
    }
}
