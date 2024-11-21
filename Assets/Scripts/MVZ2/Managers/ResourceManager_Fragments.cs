using System.Linq;
using MVZ2Logic.Entities;
using PVZEngine;
using UnityEngine;

namespace MVZ2.Managers
{
    public partial class ResourceManager : MonoBehaviour
    {
        #region 元数据
        public FragmentMetaList GetFragmentMetaList(string nsp)
        {
            var modResource = GetModResource(nsp);
            if (modResource == null)
                return null;
            return modResource.FragmentMetaList;
        }
        #endregion

        #region 碎片渐变
        public Gradient GetFragmentGradient(NamespaceID id)
        {
            var meta = GetFragmentMetaList(id.spacename);
            if (meta == null)
                return null;
            return meta.metas.FirstOrDefault(m => m.name == id.path)?.gradient;
        }
        #endregion
    }
}
