using PVZEngine;

namespace MVZ2Logic.Saves
{
    public interface ISaveDataProvider
    {
        bool IsUnlocked(NamespaceID unlockID);
        void Unlock(NamespaceID unlockID);
        T GetModSaveData<T>(string spaceName);
        ModSaveData GetModSaveData(string spaceName);
    }
}
