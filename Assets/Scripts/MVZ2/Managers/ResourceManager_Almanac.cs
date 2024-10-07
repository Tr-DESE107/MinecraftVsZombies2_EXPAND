using System.Linq;
using MVZ2.GameContent;
using MVZ2.Resources;
using MVZ2.UI;
using PVZEngine;
using PVZEngine.Definitions;
using PVZEngine.Level;
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
            var metaList = GetAlmanacMetaList(id.spacename);
            if (metaList == null)
                return null;
            if (!metaList.entries.TryGetValue(type, out var entries))
                return null;
            return entries.FirstOrDefault(e => e.id == id);
        }
    }
}
