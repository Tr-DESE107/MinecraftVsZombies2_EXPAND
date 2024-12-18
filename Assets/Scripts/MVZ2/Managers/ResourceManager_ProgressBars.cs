using System.Linq;
using System.Threading.Tasks;
using MVZ2.Metas;
using PVZEngine;
using UnityEngine;

namespace MVZ2.Managers
{
    public partial class ResourceManager : MonoBehaviour
    {
        public ProgressBarMetaList GetProgressBarMetaList(string spaceName)
        {
            var modResource = GetModResource(spaceName);
            if (modResource == null)
                return null;
            return modResource.ProgressBarMetaList;
        }
        public ProgressBarMeta[] GetModProgressBarMetas(string spaceName)
        {
            var barMetalist = GetProgressBarMetaList(spaceName);
            if (barMetalist == null)
                return null;
            return barMetalist.metas.ToArray();
        }
        public ProgressBarMeta GetProgressBarMeta(NamespaceID progressBarID)
        {
            if (progressBarID == null)
                return null;
            var barMetalist = GetProgressBarMetaList(progressBarID.spacename);
            if (barMetalist == null)
                return null;
            return barMetalist.metas.FirstOrDefault(m => m.ID == progressBarID.path);
        }
    }
}
