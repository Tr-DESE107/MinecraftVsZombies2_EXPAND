using MVZ2Logic.Saves;
using PVZEngine;

namespace MVZ2Logic.Games
{
    public interface IGameSaveData
    {
        bool IsUnlocked(NamespaceID unlockID);
        void Unlock(NamespaceID unlockID);
        NamespaceID[] GetUnlockedEnemies();
        T GetModSaveData<T>(string spaceName);
        ModSaveData GetModSaveData(string spaceName);
    }
}
