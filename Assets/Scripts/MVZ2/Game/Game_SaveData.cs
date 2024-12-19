using MVZ2Logic.Games;
using MVZ2Logic.Saves;
using PVZEngine;

namespace MVZ2.Games
{
    public partial class Game
    {
        public bool IsUnlocked(NamespaceID unlockID)
        {
            return saveDataProvider.IsUnlocked(unlockID);
        }

        public void Unlock(NamespaceID unlockID)
        {
            saveDataProvider.Unlock(unlockID);
        }

        public NamespaceID[] GetUnlockedEnemies()
        {
            return saveDataProvider.GetUnlockedEnemies();
        }

        public T GetModSaveData<T>(string spaceName)
        {
            return saveDataProvider.GetModSaveData<T>(spaceName);
        }

        public ModSaveData GetModSaveData(string spaceName)
        {
            return saveDataProvider.GetModSaveData(spaceName);
        }

        private IGameSaveData saveDataProvider;
    }
}