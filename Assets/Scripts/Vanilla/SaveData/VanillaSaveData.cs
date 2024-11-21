using System;
using MVZ2Logic.Saves;
using PVZEngine;
using UnityEngine;

namespace MVZ2.Vanilla.Save
{
    public class VanillaSaveData : ModSaveData, IBuiltinSaveData
    {
        public VanillaSaveData(string spaceName) : base(spaceName)
        {
        }
        protected override SerializableModSaveData CreateSerializable()
        {
            return new SerializableVanillaSaveData()
            {
                lastMapID = LastMapID,
                mapTalkID = MapTalkID,
                money = money,
                blueprintSlots = blueprintSlots,
                starshardSlots = starshardSlots,
            };
        }
        public void LoadSerializable(SerializableVanillaSaveData serializable)
        {
            LoadFromSerializable(serializable);
            LastMapID = serializable.lastMapID;
            MapTalkID = serializable.mapTalkID;
            money = serializable.money;
            blueprintSlots = Mathf.Max(MIN_BLUEPRINT_SLOTS, serializable.blueprintSlots);
            starshardSlots = Mathf.Max(MIN_STARSHARD_SLOTS, serializable.starshardSlots);
        }

        public int GetMoney()
        {
            return money;
        }
        public void SetMoney(int value)
        {
            money = Math.Clamp(value, 0, 999990);
        }
        public int GetBlueprintSlots()
        {
            return blueprintSlots;
        }
        public void SetBlueprintSlots(int value)
        {
            blueprintSlots = value;
        }
        public int GetStarshardSlots()
        {
            return starshardSlots;
        }
        public void SetStarshardSlots(int value)
        {
            starshardSlots = value;
        }
        public const int MIN_BLUEPRINT_SLOTS = 6;
        public const int MIN_STARSHARD_SLOTS = 3;
        public NamespaceID LastMapID { get; set; }
        public NamespaceID MapTalkID { get; set; }
        private int money;
        private int blueprintSlots = MIN_BLUEPRINT_SLOTS;
        private int starshardSlots = MIN_STARSHARD_SLOTS;
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
        public NamespaceID mapTalkID;
        public int money;
        public int blueprintSlots;
        public int starshardSlots;
    }
}
