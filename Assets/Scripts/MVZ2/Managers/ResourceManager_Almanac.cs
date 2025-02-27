using System.Linq;
using MVZ2.Metas;
using MVZ2.Vanilla.Almanacs;
using PVZEngine;
using UnityEngine;

namespace MVZ2.Managers
{
    public partial class ResourceManager : MonoBehaviour
    {
        public AlmanacMetaList GetAlmanacMetaList(string nsp)
        {
            var modResource = GetModResource(nsp);
            if (modResource == null)
                return null;
            return modResource.AlmanacMetaList;
        }
        public AlmanacMetaEntry GetAlmanacMetaEntry(string type, NamespaceID id)
        {
            if (!NamespaceID.IsValid(id))
                return null;
            var metaList = GetAlmanacMetaList(id.SpaceName);
            if (metaList == null)
                return null;
            if (!metaList.TryGetCategory(type, out var entries))
                return null;
            return entries.entries.FirstOrDefault(e => e.id == id);
        }
        public bool IsContraptionInAlmanac(NamespaceID id)
        {
            return GetAlmanacMetaEntry(VanillaAlmanacCategories.CONTRAPTIONS, id) != null;
        }
        public bool IsEnemyInAlmanac(NamespaceID id)
        {
            return GetAlmanacMetaEntry(VanillaAlmanacCategories.ENEMIES, id) != null;
        }
    }
}
