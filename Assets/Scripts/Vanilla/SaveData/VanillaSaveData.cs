using System;
using MVZ2.Save;
using PVZEngine;

namespace MVZ2.Vanilla.Save
{
    public class VanillaSaveData : ModSaveData, ILastMapSaveData, IMoneySaveData
    {
        public VanillaSaveData(string spaceName) : base(spaceName)
        {
        }
        protected override SerializableModSaveData CreateSerializable()
        {
            return new SerializableVanillaSaveData()
            {
                lastMapID = LastMapID,
                money = money
            };
        }
        public void LoadSerializable(SerializableVanillaSaveData serializable)
        {
            LoadFromSerializable(serializable);
            LastMapID = serializable.lastMapID;
            money = serializable.money;
        }

        public int GetMoney()
        {
            return money;
        }

        public void SetMoney(int value)
        {
            money = Math.Clamp(value, 0, 999990);
        }
        public NamespaceID LastMapID { get; set; }
        private int money;
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
        public int money;
    }
}
