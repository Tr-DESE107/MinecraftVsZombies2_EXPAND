using System.Linq;
using PVZEngine;
using PVZEngine.Definitions;
using PVZEngine.Level;
using UnityEngine;

namespace MVZ2
{
    public class SaveManager : MonoBehaviour
    {
        public bool IsPrologueCleared()
        {
            return false;
        }
        public void LoadSaveData()
        {
            saveData = new SaveData();
        }
        public NamespaceID GetDifficulty()
        {
            return saveData.difficulty;
        }
        public void SetDifficulty(NamespaceID difficulty)
        {
            saveData.difficulty = difficulty;
        }
        public NamespaceID[] GetUnlockedContraptions()
        {
            var contraptions = MainManager.Instance.Game.GetDefinitions<EntityDefinition>().Where(d => d.Type == EntityTypes.PLANT);
            return contraptions.Select(c => c.GetID()).ToArray();
        }
        public MainManager Main => main;
        [SerializeField]
        private MainManager main;
        private SaveData saveData;
    }
}
