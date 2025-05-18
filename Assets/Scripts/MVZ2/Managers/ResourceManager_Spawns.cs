using System.Linq;
using MVZ2.Metas;
using PVZEngine;
using UnityEngine;

namespace MVZ2.Managers
{
    public partial class ResourceManager : MonoBehaviour
    {
        #region 生成
        public SpawnMetaList GetSpawnMetaList(string spaceName)
        {
            var modResource = GetModResource(spaceName);
            if (modResource == null)
                return null;
            return modResource.SpawnMetaList;
        }
        public SpawnMeta[] GetModSpawnMetas(string spaceName)
        {
            var metalist = GetSpawnMetaList(spaceName);
            if (metalist == null)
                return null;
            return metalist.Metas.ToArray();
        }
        public SpawnMeta GetSpawnMeta(NamespaceID id)
        {
            if (id == null)
                return null;
            var metalist = GetSpawnMetaList(id.SpaceName);
            if (metalist == null)
                return null;
            return metalist.Metas.FirstOrDefault(m => m.ID == id.Path);
        }
        #endregion
    }
}
