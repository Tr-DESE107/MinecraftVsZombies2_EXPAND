﻿using System;

namespace MVZ2.Saves
{
    public class UserDataItem
    {
        public SerializableSaveDataMeta ToSerializable()
        {
            return new SerializableSaveDataMeta()
            {
                username = Username
            };
        }
        public static UserDataItem FromSerializable(SerializableSaveDataMeta serializable)
        {
            if (serializable == null)
                return null;
            return new UserDataItem()
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
