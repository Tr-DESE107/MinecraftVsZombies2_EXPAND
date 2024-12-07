using System;
using PVZEngine;

namespace MVZ2Logic.Saves
{
    public class UserStatEntry
    {
        public UserStatEntry(NamespaceID id)
        {
            ID = id;
        }
        public SerializableUserStatEntry ToSerializable()
        {
            return new SerializableUserStatEntry()
            {
                id = ID,
                value = Value
            };
        }
        public static UserStatEntry FromSerializable(SerializableUserStatEntry serializable)
        {
            return new UserStatEntry(serializable.id)
            {
                Value = serializable.value
            };
        }
        public NamespaceID ID { get; private set; }
        public long Value { get; set; }
    }
    [Serializable]
    public class SerializableUserStatEntry
    {
        public NamespaceID id;
        public long value;
    }
}
