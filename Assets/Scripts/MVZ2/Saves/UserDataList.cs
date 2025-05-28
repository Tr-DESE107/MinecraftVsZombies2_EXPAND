﻿using System;
using System.Linq;

namespace MVZ2.Saves
{
    public class UserDataList
    {
        public UserDataList(int metaCount)
        {
            metas = new UserDataItem[metaCount];
        }
        public UserDataItem Create(int index)
        {
            if (index < 0 || index >= metas.Length)
                return null;
            var meta = new UserDataItem();
            metas[index] = meta;
            return meta;
        }
        public UserDataItem Get(int index)
        {
            if (index < 0 || index >= metas.Length)
                return null;
            return metas[index];
        }
        public bool Delete(int index)
        {
            if (index < 0 || index >= metas.Length)
                return false;
            metas[index] = null;
            return true;
        }
        public UserDataItem[] GetAllUsers()
        {
            return metas.ToArray();
        }
        public int GetMaxUserCount()
        {
            return metas.Length;
        }
        public SerializableUserDataList ToSerializable()
        {
            return new SerializableUserDataList()
            {
                currentUserIndex = CurrentUserIndex,
                metas = metas.Select(m => m != null ? m.ToSerializable() : null).ToArray()
            };
        }
        public static UserDataList FromSerializable(SerializableUserDataList serializable)
        {
            var metaList = new UserDataList(serializable.metas.Length);
            metaList.CurrentUserIndex = Math.Clamp(serializable.currentUserIndex, 0, serializable.metas.Length - 1);
            for (int i = 0; i < metaList.metas.Length; i++)
            {
                var seriMeta = serializable.metas[i];
                if (seriMeta == null)
                    continue;
                metaList.metas[i] = UserDataItem.FromSerializable(seriMeta);
            }
            return metaList;
        }
        public int CurrentUserIndex { get; set; }
        private UserDataItem[] metas;
    }
    [Serializable]
    public class SerializableUserDataList
    {
        public int currentUserIndex;
        public SerializableSaveDataMeta[] metas;
    }
}
