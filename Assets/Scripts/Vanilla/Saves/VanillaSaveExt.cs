using System.Collections.Generic;
using System.Linq;
using MVZ2.GameContent.Contraptions;
using MVZ2Logic;
using MVZ2Logic.Callbacks;
using MVZ2Logic.Games;
using PVZEngine;
using PVZEngine.Callbacks;

namespace MVZ2.Vanilla.Saves
{
    public static class VanillaSaveExt
    {
        public static VanillaSaveData GetVanillaSaveData(this IGlobalSaveData save)
        {
            return save.GetModSaveData<VanillaSaveData>(Global.BuiltinNamespace);
        }
        public static NamespaceID GetLastMapID(this IGlobalSaveData save)
        {
            var saveData = save.GetVanillaSaveData();
            if (saveData == null)
                return null;
            return saveData.LastMapID;
        }
        public static void SetLastMapID(this IGlobalSaveData save, NamespaceID value)
        {
            var saveData = save.GetVanillaSaveData();
            if (saveData == null)
                return;
            saveData.LastMapID = value;
        }
        public static BlueprintSelection GetLastSelection(this IGlobalSaveData save)
        {
            var saveData = save.GetVanillaSaveData();
            if (saveData == null)
                return null;
            return saveData.LastSelection;
        }
        public static void SetLastSelection(this IGlobalSaveData save, BlueprintSelection value)
        {
            var saveData = save.GetVanillaSaveData();
            if (saveData == null)
                return;
            saveData.LastSelection = value;
        }
        public static void SetMoney(this IGlobalSaveData save, int money)
        {
            var saveData = save.GetVanillaSaveData();
            if (saveData == null)
                return;
            saveData.SetMoney(money);
        }
        public static void AddMoney(this IGlobalSaveData save, int money)
        {
            save.SetMoney(save.GetMoney() + money);
        }
        public static int GetMoney(this IGlobalSaveData save)
        {
            var saveData = save.GetVanillaSaveData();
            if (saveData == null)
                return 0;
            return saveData.GetMoney();
        }
        public static void SetMapTalk(this IGlobalSaveData save, NamespaceID value)
        {
            var saveData = save.GetVanillaSaveData();
            if (saveData == null)
                return;
            saveData.MapTalkID = value;
        }
        public static NamespaceID GetMapTalk(this IGlobalSaveData save)
        {
            var saveData = save.GetVanillaSaveData();
            if (saveData == null)
                return null;
            return saveData.MapTalkID;
        }
        public static int GetBlueprintSlots(this IGlobalSaveData save)
        {
            var saveData = save.GetVanillaSaveData();
            int count = 6;
            if (saveData != null)
            {
                count = saveData.GetBlueprintSlots();
            }
            var result = new CallbackResult(count);
            Global.Game.RunCallbackWithResult(LogicCallbacks.GET_BLUEPRINT_SLOT_COUNT, new EmptyCallbackParams(), result);
            return result.GetValue<int>();
        }
        public static int GetArtifactSlots(this IGlobalSaveData save)
        {
            var saveData = save.GetVanillaSaveData();
            if (saveData == null)
                return 1;
            return saveData.GetArtifactSlots();
        }
        public static int GetStarshardSlots(this IGlobalSaveData save)
        {
            var saveData = save.GetVanillaSaveData();
            if (saveData == null)
                return 3;
            return saveData.GetStarshardSlots();
        }
        public static bool IsLevelCleared(this IGlobalSaveData save, NamespaceID stageID)
        {
            return save.IsUnlocked(GetLevelClearUnlockID(stageID));
        }
        public static bool IsAlmanacUnlocked(this IGlobalSaveData save)
        {
            return save.IsUnlocked(VanillaUnlockID.almanac);
        }
        public static bool IsStoreUnlocked(this IGlobalSaveData save)
        {
            return save.IsUnlocked(VanillaUnlockID.store);
        }
        public static bool IsMusicRoomUnlocked(this IGlobalSaveData save)
        {
            return save.IsUnlocked(VanillaUnlockID.musicRoom);
        }
        public static bool IsArcadeUnlocked(this IGlobalSaveData save)
        {
            return save.IsUnlocked(VanillaUnlockID.arcade);
        }
        public static bool IsGensokyoUnlocked(this IGlobalSaveData save)
        {
            return save.IsUnlocked(VanillaUnlockID.gensokyo);
        }
        public static bool IsTriggerUnlocked(this IGlobalSaveData save)
        {
            return save.IsUnlocked(VanillaUnlockID.trigger);
        }
        public static bool IsStarshardUnlocked(this IGlobalSaveData save)
        {
            return save.IsUnlocked(VanillaUnlockID.starshard);
        }
        public static bool IsCommandBlockUnlocked(this IGlobalSaveData save)
        {
            return save.IsContraptionUnlocked(VanillaContraptionID.commandBlock);
        }
        public static bool IsValidAndLocked(this IGlobalSaveData save, NamespaceID unlockId)
        {
            return NamespaceID.IsValid(unlockId) && !save.IsUnlocked(unlockId);
        }
        public static bool IsInvalidOrUnlocked(this IGlobalSaveData save, NamespaceID unlockId)
        {
            return !save.IsValidAndLocked(unlockId);
        }
        public static bool IsAllInvalidOrUnlocked(this IGlobalSaveData save, IEnumerable<NamespaceID> unlocks)
        {
            if (unlocks == null || unlocks.Count() <= 0)
                return true;
            return unlocks.All(u => save.IsInvalidOrUnlocked(u));
        }
        /// <summary>
        /// 梦境世界是否是梦魇状态。
        /// </summary>
        /// <param name="save"></param>
        /// <returns></returns>
        public static bool DreamIsNightmare(this IGlobalSaveData save)
        {
            // 玩家设置。
            return save.IsUnlocked(VanillaUnlockID.dreamIsNightmare);
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
