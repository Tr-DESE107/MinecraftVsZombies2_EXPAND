using MVZ2Logic;
using MVZ2Logic.Games;
using MVZ2Logic.Saves;
using PVZEngine;

namespace MVZ2.Vanilla.Saves
{
    public static class VanillaSaveExt
    {
        public static VanillaSaveData GetVanillaSaveData(this IGameSaveData save)
        {
            return save.GetModSaveData<VanillaSaveData>(Global.BuiltinNamespace);
        }
        public static NamespaceID GetLastMapID(this IGameSaveData save)
        {
            var saveData = save.GetVanillaSaveData();
            if (saveData == null)
                return null;
            return saveData.LastMapID;
        }
        public static void SetMoney(this IGameSaveData save, int money)
        {
            var saveData = save.GetVanillaSaveData();
            if (saveData == null)
                return;
            saveData.SetMoney(money);
        }
        public static void AddMoney(this IGameSaveData save, int money)
        {
            save.SetMoney(save.GetMoney() + money);
        }
        public static int GetMoney(this IGameSaveData save)
        {
            var saveData = save.GetVanillaSaveData();
            if (saveData == null)
                return 0;
            return saveData.GetMoney();
        }
        public static void SetMapTalk(this IGameSaveData save, NamespaceID value)
        {
            var saveData = save.GetVanillaSaveData();
            if (saveData == null)
                return;
            saveData.MapTalkID = value;
        }
        public static NamespaceID GetMapTalk(this IGameSaveData save)
        {
            var saveData = save.GetVanillaSaveData();
            if (saveData == null)
                return null;
            return saveData.MapTalkID;
        }
        public static void SetBlueprintSlots(this IGameSaveData save, int slots)
        {
            var saveData = save.GetVanillaSaveData();
            if (saveData == null)
                return;
            saveData.SetBlueprintSlots(slots);
        }
        public static int GetBlueprintSlots(this IGameSaveData save)
        {
            var saveData = save.GetVanillaSaveData();
            if (saveData == null)
                return 6;
            return saveData.GetBlueprintSlots();
        }
        public static int GetStarshardSlots(this IGameSaveData save)
        {
            var saveData = save.GetVanillaSaveData();
            if (saveData == null)
                return 3;
            return saveData.GetStarshardSlots();
        }
        public static void SetStarshardSlots(this IGameSaveData save, int slots)
        {
            var saveData = save.GetVanillaSaveData();
            if (saveData == null)
                return;
            saveData.SetStarshardSlots(slots);
        }
        public static bool IsLevelCleared(this IGameSaveData save, NamespaceID stageID)
        {
            return save.IsUnlocked(GetLevelClearUnlockID(stageID));
        }
        public static bool IsAlmanacUnlocked(this IGameSaveData save)
        {
            return save.IsUnlocked(VanillaUnlockID.almanac);
        }
        public static bool IsStoreUnlocked(this IGameSaveData save)
        {
            return save.IsUnlocked(VanillaUnlockID.store);
        }
        public static bool IsTriggerUnlocked(this IGameSaveData save)
        {
            return save.IsUnlocked(VanillaUnlockID.trigger);
        }
        public static bool IsStarshardUnlocked(this IGameSaveData save)
        {
            return save.IsUnlocked(VanillaUnlockID.starshard);
        }
        public static NamespaceID GetLevelClearUnlockID(NamespaceID stageID)
        {
            return new NamespaceID(stageID.spacename, $"level.{stageID.path}");
        }
    }
}
