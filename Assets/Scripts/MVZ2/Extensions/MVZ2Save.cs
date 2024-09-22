using MVZ2.GameContent;
using MVZ2.Save;
using PVZEngine;

namespace MVZ2
{
    public static class MVZ2Save
    {
        public static NamespaceID GetLastMapID(this SaveManager save)
        {
            var saveData = save.GetModSaveData<ILastMapSaveData>(save.Main.BuiltinNamespace);
            if (saveData == null)
                return null;
            return saveData.LastMapID;
        }
        public static void SetMoney(this SaveManager save, int money)
        {
            var nsp = save.Main.BuiltinNamespace;
            var saveData = save.GetModSaveData<IMoneySaveData>(nsp);
            if (saveData == null)
                return;
            saveData.SetMoney(money);
            save.SaveCurrentModData(nsp);
        }
        public static int GetMoney(this SaveManager save)
        {
            var saveData = save.GetModSaveData<IMoneySaveData>(save.Main.BuiltinNamespace);
            if (saveData == null)
                return 0;
            return saveData.GetMoney();
        }
        public static bool IsTriggerUnlocked(this SaveManager save)
        {
            return save.IsUnlocked(BuiltinUnlockID.trigger);
        }
        public static bool IsStarshardUnlocked(this SaveManager save)
        {
            return save.IsUnlocked(BuiltinUnlockID.starshard);
        }
    }
}
