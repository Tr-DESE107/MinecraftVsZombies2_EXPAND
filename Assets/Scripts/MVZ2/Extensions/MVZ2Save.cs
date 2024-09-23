using MVZ2.GameContent;
using MVZ2.Level;
using MVZ2.Save;
using PVZEngine;

namespace MVZ2.Extensions
{
    public static class MVZ2Save
    {
        public static IBuiltinSaveData GetBuiltinMapSaveData(this ISaveDataProvider save)
        {
            return save.GetModSaveData<IBuiltinSaveData>(Global.BuiltinNamespace);
        }
        public static NamespaceID GetLastMapID(this ISaveDataProvider save)
        {
            var saveData = save.GetBuiltinMapSaveData();
            if (saveData == null)
                return null;
            return saveData.LastMapID;
        }
        public static void SetMoney(this ISaveDataProvider save, int money)
        {
            var saveData = save.GetBuiltinMapSaveData();
            if (saveData == null)
                return;
            saveData.SetMoney(money);
            save.SaveCurrentModData(saveData.Namespace);
        }
        public static void AddMoney(this ISaveDataProvider save, int money)
        {
            save.SetMoney(save.GetMoney() + money);
        }
        public static int GetMoney(this ISaveDataProvider save)
        {
            var saveData = save.GetBuiltinMapSaveData();
            if (saveData == null)
                return 0;
            return saveData.GetMoney();
        }
        public static void SetBlueprintSlots(this ISaveDataProvider save, int slots)
        {
            var saveData = save.GetBuiltinMapSaveData();
            if (saveData == null)
                return;
            saveData.SetBlueprintSlots(slots);
            save.SaveCurrentModData(saveData.Namespace);
        }
        public static int GetBlueprintSlots(this ISaveDataProvider save)
        {
            var saveData = save.GetBuiltinMapSaveData();
            if (saveData == null)
                return 0;
            return saveData.GetBlueprintSlots();
        }
        public static bool IsLevelCleared(this ISaveDataProvider save, NamespaceID stageID)
        {
            return save.IsUnlocked(LevelManager.GetLevelClearUnlockID(stageID));
        }
        public static bool IsTriggerUnlocked(this ISaveDataProvider save)
        {
            return save.IsUnlocked(BuiltinUnlockID.trigger);
        }
        public static bool IsStarshardUnlocked(this ISaveDataProvider save)
        {
            return save.IsUnlocked(BuiltinUnlockID.starshard);
        }
    }
}
