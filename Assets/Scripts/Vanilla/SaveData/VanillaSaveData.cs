using System;
using MVZ2.Save;
using PVZEngine;

namespace MVZ2.Vanilla.Save
{
    public class VanillaSaveData : ModSaveData, ILastMapSaveData
    {
        public VanillaSaveData(string spaceName) : base(spaceName)
        {
        }
        protected override SerializableModSaveData CreateSerializable()
        {
            return new SerializableVanillaSaveData()
            {
                lastMapID = LastMapID
            };
        }
        public void LoadSerializable(SerializableVanillaSaveData serializable)
        {
            LoadFromSerializable(serializable);
            LastMapID = serializable.lastMapID;
        }

        public NamespaceID LastMapID { get; set; }
    }
    [Serializable]
    public class SerializableVanillaSaveData : SerializableModSaveData
    {
        public VanillaSaveData Deserialize()
        {
            var saveData = new VanillaSaveData(spaceName);
            saveData.LoadSerializable(this);
            return saveData;
        }
        public NamespaceID lastMapID;
    }
}
