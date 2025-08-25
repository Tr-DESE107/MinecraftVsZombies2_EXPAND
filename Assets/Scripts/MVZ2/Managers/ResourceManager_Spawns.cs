using System.Linq;
using MVZ2.Metas;
using UnityEngine;

namespace MVZ2.Managers
{
    public partial class ResourceManager : MonoBehaviour
    {
        #region 生成
        public SpawnMeta[] GetModSpawnMetas(string spaceName)
        {
            var resources = GetModResource(spaceName);
            if (resources == null)
                return null;
            return resources.SpawnMetaList.Metas.ToArray();
        }
        #endregion
    }
}
