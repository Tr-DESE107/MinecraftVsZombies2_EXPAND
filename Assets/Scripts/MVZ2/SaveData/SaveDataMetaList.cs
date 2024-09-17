using System;
using System.Collections.Generic;
using System.Linq;
using PVZEngine;

namespace MVZ2.Save
{
    public class SaveDataMetaList
    {
        public SaveDataMetaList(int metaCount)
        {
            metas = new SaveDataMeta[metaCount];
        }
        public SaveDataMeta CreateUserMeta(int index)
        {
            if (index < 0 || index >= metas.Length)
                return null;
            var meta = new SaveDataMeta();
            metas[index] = meta;
            return meta;
        }
        public SaveDataMeta GetUserMeta(int index)
        {
            if (index < 0 || index >= metas.Length)
                return null;
            return metas[index];
        }
        public int GetMaxUserCount()
        {
            return metas.Length;
        }
        public SerializableSaveDataMetaList ToSerializable()
        {
            return new SerializableSaveDataMetaList()
            {
                currentUserIndex = CurrentUserIndex,
                metas = metas.Select(m => m != null ? m.ToSerializable() : null).ToArray()
            };
        }
        public static SaveDataMetaList FromSerializable(SerializableSaveDataMetaList serializable)
        {
            var metaList = new SaveDataMetaList(serializable.metas.Length);
            metaList.CurrentUserIndex = serializable.currentUserIndex;
            for (int i = 0; i < metaList.metas.Length; i++)
            {
                var seriMeta = serializable.metas[i];
                if (seriMeta == null)
                    continue;
                metaList.metas[i] = SaveDataMeta.FromSerializable(seriMeta);
            }
            return metaList;
        }
        public int CurrentUserIndex { get; set; }
        private SaveDataMeta[] metas;
    }
    [Serializable]
    public class SerializableSaveDataMetaList
    {
        public int currentUserIndex;
        public SerializableSaveDataMeta[] metas;
    }
}
