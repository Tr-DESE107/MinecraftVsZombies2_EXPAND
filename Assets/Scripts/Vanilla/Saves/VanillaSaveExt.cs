using MVZ2Logic;
using MVZ2Logic.Saves;
using PVZEngine;

namespace MVZ2.Vanilla.Saves
{
    public static class VanillaSaveExt
    {
        public static VanillaSaveData GetVanillaSaveData(this ISaveDataProvider save)
        {
            return save.GetModSaveData<VanillaSaveData>(Global.BuiltinNamespace);
        }
        public static NamespaceID GetLastMapID(this ISaveDataProvider save)
        {
            var saveData = save.GetVanillaSaveData();
            if (saveData == null)
                return null;
            return saveData.LastMapID;
        }
        public static void SetMoney(this ISaveDataProvider save, int money)
        {
            var saveData = save.GetVanillaSaveData();
            if (saveData == null)
                return;
            saveData.SetMoney(money);
        }
        public static void AddMoney(this ISaveDataProvider save, int money)
        {
            save.SetMoney(save.GetMoney() + money);
        }
        public static int GetMoney(this ISaveDataProvider save)
        {
            var saveData = save.GetVanillaSaveData();
            if (saveData == null)
                return 0;
            return saveData.GetMoney();
        }
        public static void SetMapTalk(this ISaveDataProvider save, NamespaceID value)
        {
            var saveData = save.GetVanillaSaveData();
            if (saveData == null)
                return;
            saveData.MapTalkID = value;
        }
        public static NamespaceID GetMapTalk(this ISaveDataProvider save)
        {
            var saveData = save.GetVanillaSaveData();
            if (saveData == null)
                return null;
            return saveData.MapTalkID;
        }
        public static void SetBlueprintSlots(this ISaveDataProvider save, int slots)
        {
            var saveData = save.GetVanillaSaveData();
            if (saveData == null)
                return;
            saveData.SetBlueprintSlots(slots);
        }
        public static int GetBlueprintSlots(this ISaveDataProvider save)
        {
            var saveData = save.GetVanillaSaveData();
            if (saveData == null)
                return 6;
            return saveData.GetBlueprintSlots();
        }
        public static int GetStarshardSlots(this ISaveDataProvider save)
        {
            var saveData = save.GetVanillaSaveData();
            if (saveData == null)
                return 3;
            return saveData.GetStarshardSlots();
        }
        public static void SetStarshardSlots(this ISaveDataProvider save, int slots)
        {
            var saveData = save.GetVanillaSaveData();
            if (saveData == null)
                return;
            saveData.SetStarshardSlots(slots);
        }
        public static bool IsLevelCleared(this ISaveDataProvider save, NamespaceID stageID)
        {
            return save.IsUnlocked(GetLevelClearUnlockID(stageID));
        }
        public static bool IsAlmanacUnlocked(this ISaveDataProvider save)
        {
            return save.IsUnlocked(VanillaUnlockID.almanac);
        }
        public static bool IsStoreUnlocked(this ISaveDataProvider save)
        {
            return save.IsUnlocked(VanillaUnlockID.store);
        }
        public static bool IsTriggerUnlocked(this ISaveDataProvider save)
        {
            return save.IsUnlocked(VanillaUnlockID.trigger);
        }
        public static bool IsStarshardUnlocked(this ISaveDataProvider save)
        {
            return save.IsUnlocked(VanillaUnlockID.starshard);
        }
        public static NamespaceID GetLevelClearUnlockID(NamespaceID stageID)
        {
            return new NamespaceID(stageID.spacename, $"level.{stageID.path}");
        }
    }
}
