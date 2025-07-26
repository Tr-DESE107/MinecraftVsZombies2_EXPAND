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
        public void Relock(NamespaceID unlockID)
        {
            saveDataProvider.Relock(unlockID);
        }

        public bool IsContraptionUnlocked(NamespaceID id)
        {
            return saveDataProvider.IsContraptionUnlocked(id);
        }
        public NamespaceID[] GetUnlockedContraptions()
        {
            return saveDataProvider.GetUnlockedContraptions();
        }

        public bool IsEnemyUnlocked(NamespaceID id)
        {
            return saveDataProvider.IsEnemyUnlocked(id);
        }
        public NamespaceID[] GetUnlockedEnemies()
        {
            return saveDataProvider.GetUnlockedEnemies();
        }

        public bool IsArtifactUnlocked(NamespaceID id)
        {
            return saveDataProvider.IsArtifactUnlocked(id);
        }
        public NamespaceID[] GetUnlockedArtifacts()
        {
            return saveDataProvider.GetUnlockedArtifacts();
        }


        public T GetModSaveData<T>(string spaceName)
        {
            return saveDataProvider.GetModSaveData<T>(spaceName);
        }

        public ModSaveData GetModSaveData(string spaceName)
        {
            return saveDataProvider.GetModSaveData(spaceName);
        }
        public string GetCurrentUserName()
        {
            return saveDataProvider.GetCurrentUserName();
        }
        public void SaveToFile()
        {
            saveDataProvider.SaveToFile();
        }

        private IGameSaveData saveDataProvider;
    }
}