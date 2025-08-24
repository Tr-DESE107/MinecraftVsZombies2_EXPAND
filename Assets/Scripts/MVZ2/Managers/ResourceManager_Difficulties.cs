using MVZ2.Metas;
using UnityEngine;

namespace MVZ2.Managers
{
    public partial class ResourceManager : MonoBehaviour
    {
        #region 元数据列表
        public DifficultyMeta[] GetModDifficultyMetas(string nsp)
        {
            var modResource = main.ResourceManager.GetModResource(nsp);
            if (modResource == null)
                return null;
            return modResource.DifficultyMetaList.metas;
        }
        #endregion

    }
}
