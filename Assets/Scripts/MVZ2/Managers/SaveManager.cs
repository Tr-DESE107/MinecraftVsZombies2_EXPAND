using System.Collections.Generic;
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
            saveData.difficulty = new NamespaceID(Main.BuiltinNamespace, "normal");
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
            var resourceManager = Main.ResourceManager;
            var entitiesID = resourceManager.GetAllEntitiesID();
            List<NamespaceID> entities = new List<NamespaceID>();
            foreach (var id in entitiesID)
            {
                var meta = resourceManager.GetEntityMeta(id);
                if (meta == null || meta.type != EntityTypes.PLANT)
                    continue;
                entities.Add(id);
            }
            return entities.ToArray();
        }
        public MainManager Main => main;
        [SerializeField]
        private MainManager main;
        private SaveData saveData;
    }
}
