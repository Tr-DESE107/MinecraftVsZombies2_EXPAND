using System;
using System.Collections.Generic;
using System.Linq;
using PVZEngine;

namespace MVZ2.Save
{
    public class SaveDataMeta
    {
        public SerializableSaveDataMeta ToSerializable()
        {
            return new SerializableSaveDataMeta()
            {
                username = Username
            };
        }
        public static SaveDataMeta FromSerializable(SerializableSaveDataMeta serializable)
        {
            if (serializable == null)
                return null;
            return new SaveDataMeta()
            {
                Username = serializable.username
            };
        }
        public string Username { get; set; }
    }
    [Serializable]
    public class SerializableSaveDataMeta
    {
        public string username;
    }
}
