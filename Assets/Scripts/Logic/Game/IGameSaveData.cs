using MVZ2Logic.Saves;
using PVZEngine;

namespace MVZ2Logic.Games
{
    public interface IGameSaveData
    {
        bool IsUnlocked(NamespaceID unlockID);
        void Unlock(NamespaceID unlockID);
        void Relock(NamespaceID unlockID);
        bool IsContraptionUnlocked(NamespaceID contraptionID);
        NamespaceID[] GetUnlockedContraptions();
        bool IsEnemyUnlocked(NamespaceID contraptionID);
        NamespaceID[] GetUnlockedEnemies();
        T GetModSaveData<T>(string spaceName);
        string GetCurrentUserName();
        ModSaveData GetModSaveData(string spaceName);
    }
}
