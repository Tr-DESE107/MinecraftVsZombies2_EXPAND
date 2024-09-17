using System;
using MVZ2.Save;

namespace MVZ2.Vanilla.Save
{
    public class VanillaSaveData : ModSaveData
    {
        public VanillaSaveData(string spaceName) : base(spaceName)
        {
        }
        protected override SerializableModSaveData CreateSerializable()
        {
            return new SerializableVanillaSaveData();
        }
        public void LoadSerializable(SerializableVanillaSaveData serializable)
        {
            LoadFromSerializable(serializable);
        }
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
    }
}
