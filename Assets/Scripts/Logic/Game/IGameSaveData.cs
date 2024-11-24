using MVZ2Logic.Entities;
using MVZ2Logic.Level;
using MVZ2Logic.Models;
using MVZ2Logic.Saves;
using PVZEngine;

namespace MVZ2Logic.Games
{
    public interface IGameSaveData
    {
        bool IsUnlocked(NamespaceID unlockID);
        void Unlock(NamespaceID unlockID);
        T GetModSaveData<T>(string spaceName);
        ModSaveData GetModSaveData(string spaceName);
    }
}
