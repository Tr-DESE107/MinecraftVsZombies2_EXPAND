using System;
using System.Collections.Generic;
using System.Linq;
using MVZ2.Managers;
using MVZ2.Metas;
using MVZ2.UI;
using MVZ2.Vanilla;
using MVZ2.Vanilla.Almanacs;
using MVZ2Logic.Callbacks;
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
        public void GetOrderedArtifacts(IEnumerable<NamespaceID> artifactID, List<NamespaceID> appendList)
        {
            var idList = GetIDListByAlmanacOrder(artifactID, VanillaAlmanacCategories.ARTIFACTS);
            var ordered = CompressLayout(idList, GetMiscCountPerRow());
            appendList.AddRange(ordered);
        }
        public void GetUnlockedMiscGroups(List<AlmanacEntryGroup> appendList)
        {
            var metaList = Main.ResourceManager.GetAlmanacMetaList(Main.BuiltinNamespace);
            var category = metaList.GetCategory(VanillaAlmanacCategories.MISC);
            var groups = category.groups.Select(g => new AlmanacEntryGroup()
            {
                name = g.name,
                entries = g.entries.Where(e => !NamespaceID.IsValid(e.unlock) || Main.SaveManager.IsUnlocked(e.unlock)).ToArray()
            }).Where(g => g != null && g.entries.Count() > 0);
            appendList.AddRange(groups);
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
        public AlmanacEntryViewData GetEnemyEntryViewData(NamespaceID id)
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
        public AlmanacEntryViewData GetArtifactEntryViewData(NamespaceID id)
        {
            if (!NamespaceID.IsValid(id))
                return AlmanacEntryViewData.Empty;
            var def = Main.Game.GetArtifactDefinition(id);
            if (def == null)
                return AlmanacEntryViewData.Empty;
            var spriteRef = def.GetSpriteReference();
            var sprite = Main.GetFinalSprite(spriteRef);
            return new AlmanacEntryViewData() { sprite = sprite };
        }
        public AlmanacEntryGroupViewData GetMiscGroupViewData(AlmanacEntryGroup group)
        {
            return new AlmanacEntryGroupViewData()
            {
                name = Main.LanguageManager._p(VanillaStrings.CONTEXT_ALMANAC_GROUP_NAME, group.name),
                entries = group.entries.Select(e =>
                {
                    var spriteID = e.sprite;
                    var modelID = e.model;
                    Sprite sprite;
                    if (NamespaceID.IsValid(modelID))
                    {
                        sprite = Main.ResourceManager.GetModelIcon(modelID);
                    }
                    else
                    {
                        sprite = Main.GetFinalSprite(spriteID);
                    }
                    return new AlmanacEntryViewData()
                    {
                        sprite = sprite
                    };
                }).ToArray()
            };
        }
        private NamespaceID[] GetIDListByAlmanacOrder(IEnumerable<NamespaceID> idList, string category)
        {
            if (idList == null || idList.Count() == 0)
                return Array.Empty<NamespaceID>();
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
        public int GetMiscCountPerRow()
        {
            return miscCountPerRow;
        }
        public MainManager Main => MainManager.Instance;
        [SerializeField]
        private int blueprintCountPerRowStandalone = 8;
        [SerializeField]
        private int blueprintCountPerRowMobile = 4;
        [SerializeField]
        private int enemyCountPerRow = 5;
        [SerializeField]
        private int miscCountPerRow = 5;
    }
    public class AlmanacEntryGroup
    {
        public string name;
        public AlmanacMetaEntry[] entries;
    }
}
