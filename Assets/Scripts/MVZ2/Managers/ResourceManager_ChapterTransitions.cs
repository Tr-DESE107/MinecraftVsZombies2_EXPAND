using System.Linq;
using MVZ2.Metas;
using PVZEngine;
using UnityEngine;

namespace MVZ2.Managers
{
    public partial class ResourceManager : MonoBehaviour
    {
        public ChapterTransitionMetaList GetChapterTransitionMetaList(string spaceName)
        {
            var modResource = GetModResource(spaceName);
            if (modResource == null)
                return null;
            return modResource.ChapterTransitionMetaList;
        }
        public ChapterTransitionMeta[] GetModChapterTransitionMetas(string spaceName)
        {
            var metalist = GetChapterTransitionMetaList(spaceName);
            if (metalist == null)
                return null;
            return metalist.Metas.ToArray();
        }
        public ChapterTransitionMeta GetChapterTransitionMeta(NamespaceID id)
        {
            if (id == null)
                return null;
            var metalist = GetChapterTransitionMetaList(id.SpaceName);
            if (metalist == null)
                return null;
            return metalist.Metas.FirstOrDefault(m => m.ID == id.Path);
        }
    }
}
