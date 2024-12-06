using System.Collections.Generic;
using System.Linq;
using MVZ2.Games;
using MVZ2.Managers;
using MVZ2.UI;
using MVZ2.Vanilla.Almanacs;
using PVZEngine;
using UnityEngine;

namespace MVZ2.Almanacs
{
    public class AlmanacManager : MonoBehaviour
    {
        public void GetOrderedBlueprints(IEnumerable<NamespaceID> blueprints, List<NamespaceID> appendList)
        {
            var idList = GetIDListByAlmanacOrder(blueprints, VanillaAlmanacCategories.CONTRAPTIONS);
            var ordered = CompressLayout(idList, GetBlueprintCountPerRow());
            appendList.AddRange(ordered);
        }
        public void GetOrderedEnemies(IEnumerable<NamespaceID> enemiesID, List<NamespaceID> appendList)
        {
            var idList = GetIDListByAlmanacOrder(enemiesID, VanillaAlmanacCategories.ENEMIES);
            var ordered = CompressLayout(idList, GetEnemyCountPerRow());
            appendList.AddRange(ordered);
        }
        public ChoosingBlueprintViewData GetChoosingBlueprintViewData(NamespaceID id)
        {
            if (!NamespaceID.IsValid(id))
                return ChoosingBlueprintViewData.Empty;
            var blueprintDef = Main.Game.GetSeedDefinition(id);
            if (blueprintDef == null)
                return ChoosingBlueprintViewData.Empty;
            return new ChoosingBlueprintViewData()
            {
                blueprint = Main.ResourceManager.GetBlueprintViewData(blueprintDef),
                disabled = false
            };
        }
        public AlmanacEntryViewData GetEnemyViewData(NamespaceID id)
        {
            if (!NamespaceID.IsValid(id))
                return AlmanacEntryViewData.Empty;
            var def = Main.Game.GetEntityDefinition(id);
            if (def == null)
                return AlmanacEntryViewData.Empty;
            var modelID = def.GetModelID();
            var modelIcon = Main.ResourceManager.GetModelIcon(modelID);
            return new AlmanacEntryViewData() { sprite = modelIcon };
        }
        private NamespaceID[] GetIDListByAlmanacOrder(IEnumerable<NamespaceID> idList, string category)
        {
            var almanacIndexes = idList.Select(id => (id, index: Main.ResourceManager.GetAlmanacMetaEntry(category, id)?.index ?? 0));
            var maxAlmanacIndex = almanacIndexes.Max(tuple => tuple.index);
            var ordered = new NamespaceID[maxAlmanacIndex + 1];
            for (int i = 0; i < ordered.Length; i++)
            {
                var tuple = almanacIndexes.FirstOrDefault(tuple => tuple.index == i);
                ordered[i] = tuple.id;
            }
            return ordered;
        }
        private IEnumerable<NamespaceID> CompressLayout(IEnumerable<NamespaceID> idList, int countPerRow)
        {
            var groups = idList
                .Select((v, i) => (v, i))
                .GroupBy(p => p.i / countPerRow);
            return groups
                .Where(g => !g.All(p => !NamespaceID.IsValid(p.v)))
                .SelectMany(g => g.Select(p => p.v))
                .ToArray();
        }
        public int GetBlueprintCountPerRow()
        {
            return Main.IsMobile() ? blueprintCountPerRowMobile : blueprintCountPerRowStandalone;
        }
        public int GetEnemyCountPerRow()
        {
            return enemyCountPerRow;
        }
        public MainManager Main => MainManager.Instance;
        [SerializeField]
        private int blueprintCountPerRowStandalone = 8;
        [SerializeField]
        private int blueprintCountPerRowMobile = 4;
        [SerializeField]
        private int enemyCountPerRow = 5;
    }
}
