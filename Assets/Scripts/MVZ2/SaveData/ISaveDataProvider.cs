using PVZEngine;

namespace MVZ2.Save
{
    public interface ISaveDataProvider
    {
        bool IsUnlocked(NamespaceID unlockID);
        void Unlock(NamespaceID unlockID);
        T GetModSaveData<T>(string spaceName);
        ModSaveData GetModSaveData(string spaceName);
    }
}
