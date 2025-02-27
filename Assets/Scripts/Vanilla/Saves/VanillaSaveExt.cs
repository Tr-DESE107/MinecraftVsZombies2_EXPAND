using MVZ2.Vanilla.Callbacks;
using MVZ2Logic;
using MVZ2Logic.Callbacks;
using MVZ2Logic.Games;
using PVZEngine;
using PVZEngine.Triggers;

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
        public static void SetLastMapID(this IGameSaveData save, NamespaceID value)
        {
            var saveData = save.GetVanillaSaveData();
            if (saveData == null)
                return;
            saveData.LastMapID = value;
        }
        public static BlueprintSelection GetLastSelection(this IGameSaveData save)
        {
            var saveData = save.GetVanillaSaveData();
            if (saveData == null)
                return null;
            return saveData.LastSelection;
        }
        public static void SetLastSelection(this IGameSaveData save, BlueprintSelection value)
        {
            var saveData = save.GetVanillaSaveData();
            if (saveData == null)
                return;
            saveData.LastSelection = value;
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
        public static int GetBlueprintSlots(this IGameSaveData save)
        {
            var saveData = save.GetVanillaSaveData();
            int count = 6;
            if (saveData != null)
            {
                count = saveData.GetBlueprintSlots();
            }
            var result = new TriggerResultInt()
            {
                Result = count,
            };
            Global.Game.RunCallback(LogicCallbacks.GET_BLUEPRINT_SLOT_COUNT, result, c => c(result));
            return result.Result;
        }
        public static int GetArtifactSlots(this IGameSaveData save)
        {
            var saveData = save.GetVanillaSaveData();
            if (saveData == null)
                return 1;
            return saveData.GetArtifactSlots();
        }
        public static int GetStarshardSlots(this IGameSaveData save)
        {
            var saveData = save.GetVanillaSaveData();
            if (saveData == null)
                return 3;
            return saveData.GetStarshardSlots();
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
        public static bool IsMusicRoomUnlocked(this IGameSaveData save)
        {
            return save.IsUnlocked(VanillaUnlockID.musicRoom);
        }
        public static bool IsGensokyoUnlocked(this IGameSaveData save)
        {
            return save.IsUnlocked(VanillaUnlockID.gensokyo);
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
            return new NamespaceID(stageID.SpaceName, GetLevelClearUnlockID(stageID.Path));
        }
        public static string GetLevelClearUnlockID(string stageID)
        {
            return $"level.{stageID}";
        }
    }
}
