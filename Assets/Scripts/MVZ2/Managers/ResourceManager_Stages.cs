using System.Linq;
using MVZ2.Metas;
using PVZEngine;
using UnityEngine;

namespace MVZ2.Managers
{
    public partial class ResourceManager : MonoBehaviour
    {
        public StageMetaList GetStageMetaList(string spaceName)
        {
            var modResource = GetModResource(spaceName);
            if (modResource == null)
                return null;
            return modResource.StageMetaList;
        }
        public StageMeta[] GetModStageMetas(string spaceName)
        {
            var stageMetalist = GetStageMetaList(spaceName);
            if (stageMetalist == null)
                return null;
            return stageMetalist.metas.ToArray();
        }
        public StageMeta GetStageMeta(NamespaceID stageID)
        {
            if (stageID == null)
                return null;
            var stageMetalist = GetStageMetaList(stageID.SpaceName);
            if (stageMetalist == null)
                return null;
            return stageMetalist.metas.FirstOrDefault(m => m.ID == stageID.Path);
        }
    }
}
