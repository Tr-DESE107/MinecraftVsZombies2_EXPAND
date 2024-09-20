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
    }
}
