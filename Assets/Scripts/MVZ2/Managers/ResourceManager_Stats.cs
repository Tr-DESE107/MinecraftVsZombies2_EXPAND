using System.Linq;
using MVZ2.Metas;
using PVZEngine;
using UnityEngine;

namespace MVZ2.Managers
{
    public partial class ResourceManager : MonoBehaviour
    {
        public StatMetaList GetStatMetaList(string spaceName)
        {
            var modResource = GetModResource(spaceName);
            if (modResource == null)
                return null;
            return modResource.StatMetaList;
        }
        public StatCategoryMeta GetStatCategoryMeta(NamespaceID categoryID)
        {
            if (categoryID == null)
                return null;
            var stageMetalist = GetStatMetaList(categoryID.spacename);
            if (stageMetalist == null)
                return null;
            return stageMetalist.categories.FirstOrDefault(m => m.ID == categoryID.path);
        }
        public string GetStatEntryName(NamespaceID entryID, StatCategoryType type)
        {
            switch (type)
            {
                case StatCategoryType.Entity:
                    return GetEntityName(entryID);
            }
            return entryID.ToString();
        }
    }
}
