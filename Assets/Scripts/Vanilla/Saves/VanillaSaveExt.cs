#nullable enable

using MVZ2.GameContent.Contraptions;
using MVZ2Logic.Games;

namespace MVZ2.Vanilla.Saves
{
    public static class VanillaSaveExt
    {
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
    }
}