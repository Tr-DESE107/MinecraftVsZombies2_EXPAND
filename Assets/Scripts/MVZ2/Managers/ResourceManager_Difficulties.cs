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
            var modResource = main.ResourceManager.GetModResource(difficulty.SpaceName);
            if (modResource == null)
                return null;
            return modResource.DifficultyMetaList.metas.FirstOrDefault(m => m.ID == difficulty.Path);
        }
        #endregion
        public string GetDifficultyName(NamespaceID difficulty)
        {
            var diffMeta = main.ResourceManager.GetDifficultyMeta(difficulty);
            string name = diffMeta != null ? diffMeta.Name : Vanilla.VanillaStrings.DIFFICULTY_UNKNOWN;
            return main.LanguageManager._p(Vanilla.VanillaStrings.CONTEXT_DIFFICULTY, name);
        }
        public NamespaceID[] GetAllDifficulties()
        {
            return difficultyCache.ToArray();
        }
        private List<NamespaceID> difficultyCache = new List<NamespaceID>();
    }
}
