using System.Collections.Generic;
using System.Linq;
using MVZ2.Talk;
using PVZEngine;
using UnityEngine;

namespace MVZ2
{
    public partial class ResourceManager : MonoBehaviour
    {
        #region 元数据列表
        public DifficultyMetaList GetDifficultyMetaList(string nsp)
        {
            var modResource = main.ResourceManager.GetModResource(nsp);
            if (modResource == null)
                return null;
            return modResource.DifficultyMetaList;
        }
        #endregion

        #region 元数据
        public DifficultyMeta GetDifficultyMeta(NamespaceID difficulty)
        {
            var modResource = main.ResourceManager.GetModResource(difficulty.spacename);
            if (modResource == null)
                return null;
            return modResource.DifficultyMetaList.metas.FirstOrDefault(m => m.name == difficulty.path);
        }
        #endregion
    }
}
