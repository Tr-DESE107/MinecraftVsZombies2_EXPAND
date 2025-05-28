using System.Linq;
using MVZ2.Metas;
using PVZEngine;
using UnityEngine;

namespace MVZ2.Managers
{
    public partial class ResourceManager : MonoBehaviour
    {
        #region 元数据列表
        public GridMetaList GetGridMetaList(string nsp)
        {
            var modResource = main.ResourceManager.GetModResource(nsp);
            if (modResource == null)
                return null;
            return modResource.GridMetaList;
        }
        #endregion

        #region 层
        public GridLayerMeta GetGridLayerMeta(NamespaceID grid)
        {
            var modResource = main.ResourceManager.GetModResource(grid.SpaceName);
            if (modResource == null)
                return null;
            return modResource.GridMetaList.layers.FirstOrDefault(m => m.ID == grid.Path);
        }
        #endregion

        #region 错误
        public GridErrorMeta GetGridErrorMeta(NamespaceID grid)
        {
            var modResource = main.ResourceManager.GetModResource(grid.SpaceName);
            if (modResource == null)
                return null;
            return modResource.GridMetaList.errors.FirstOrDefault(m => m.ID == grid.Path);
        }
        #endregion
    }
}
