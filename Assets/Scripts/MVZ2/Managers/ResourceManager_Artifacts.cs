using System;
using System.Collections.Generic;
using System.Linq;
using MVZ2.Metas;
using MVZ2.Vanilla;
using PVZEngine;
using UnityEngine;

namespace MVZ2.Managers
{
    public partial class ResourceManager : MonoBehaviour
    {
        #region 元数据列表
        public ArtifactMetaList GetArtifactMetaList(string nsp)
        {
            var modResource = main.ResourceManager.GetModResource(nsp);
            if (modResource == null)
                return null;
            return modResource.ArtifactMetaList;
        }
        public ArtifactMeta[] GetModArtifactMetas(string nsp)
        {
            var metaList = GetArtifactMetaList(nsp);
            if (metaList == null)
                return Array.Empty<ArtifactMeta>();
            return metaList.metas.ToArray();
        }
        public NamespaceID[] GetAllArtifactsID()
        {
            return artifactsCacheDict.Keys.ToArray();
        }
        #endregion

        #region 元数据
        public ArtifactMeta GetArtifactMeta(NamespaceID entityID)
        {
            return artifactsCacheDict.TryGetValue(entityID, out var meta) ? meta : null;
        }
        public string GetArtifactName(NamespaceID entityID)
        {
            if (entityID == null)
                return "null";
            var meta = GetArtifactMeta(entityID);
            if (meta == null)
                return entityID.ToString();
            var name = meta.Name ?? VanillaStrings.UNKNOWN_ARTIFACT_NAME;
            return Main.LanguageManager._p(VanillaStrings.CONTEXT_ARTIFACT_NAME, name);
        }
        public string GetArtifactTooltip(NamespaceID entityID)
        {
            if (entityID == null)
                return "null";
            var meta = GetArtifactMeta(entityID);
            if (meta == null)
                return entityID.ToString();
            var tooltip = meta.Tooltip ?? VanillaStrings.UNKNOWN_ARTIFACT_TOOLTIP;
            return Main.LanguageManager._p(VanillaStrings.CONTEXT_ARTIFACT_TOOLTIP, tooltip);
        }
        #endregion

        private Dictionary<NamespaceID, ArtifactMeta> artifactsCacheDict = new Dictionary<NamespaceID, ArtifactMeta>();
    }
}
