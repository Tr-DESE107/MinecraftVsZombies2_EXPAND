using System;

namespace MVZ2.Save
{
    public class UserStatEntry
    {
        public UserStatEntry(string id)
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
        public string ID { get; private set; }
        public long Value { get; set; }
    }
    [Serializable]
    public class SerializableUserStatEntry
    {
        public string id;
        public long value;
    }
}
