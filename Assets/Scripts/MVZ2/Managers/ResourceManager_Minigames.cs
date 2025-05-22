using System.Collections.Generic;
using System.Linq;
using MVZ2.Metas;
using PVZEngine;
using UnityEngine;

namespace MVZ2.Managers
{
    public partial class ResourceManager : MonoBehaviour
    {
        #region 元数据列表
        public MinigameMetaList GetMinigameMetaList(string nsp)
        {
            var modResource = main.ResourceManager.GetModResource(nsp);
            if (modResource == null)
                return null;
            return modResource.MinigameMetaList;
        }
        #endregion

        #region 元数据
        public MinigameMeta GetMinigameMeta(NamespaceID minigame)
        {
            if (!NamespaceID.IsValid(minigame))
                return null;
            var modResource = main.ResourceManager.GetModResource(minigame.SpaceName);
            if (modResource == null)
                return null;
            return modResource.MinigameMetaList.metas.FirstOrDefault(m => m.ID == minigame.Path);
        }
        #endregion
        public NamespaceID[] GetAllMinigames()
        {
            return minigameCache.ToArray();
        }
        private List<NamespaceID> minigameCache = new List<NamespaceID>();
    }
}
