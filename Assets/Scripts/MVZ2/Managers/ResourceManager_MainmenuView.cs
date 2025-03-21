﻿using System.Collections.Generic;
using System.Linq;
using MVZ2.Metas;
using PVZEngine;
using UnityEngine;

namespace MVZ2.Managers
{
    public partial class ResourceManager : MonoBehaviour
    {
        public MainmenuViewMetaList GetMainmenuViewMetaList(string spaceName)
        {
            var modResource = GetModResource(spaceName);
            if (modResource == null)
                return null;
            return modResource.MainmenuViewMetaList;
        }
        public MainmenuViewMeta[] GetModMainmenuViewMetas(string spaceName)
        {
            var metaList = GetMainmenuViewMetaList(spaceName);
            if (metaList == null)
                return null;
            return metaList.Metas;
        }
        public NamespaceID[] GetAllMainmenuViews()
        {
            return mainmenuViewCacheDict.Keys.ToArray();
        }
        public MainmenuViewMeta GetMainmenuViewMeta(NamespaceID id)
        {
            return mainmenuViewCacheDict.TryGetValue(id, out var meta) ? meta : null;
        }
        private Dictionary<NamespaceID, MainmenuViewMeta> mainmenuViewCacheDict = new Dictionary<NamespaceID, MainmenuViewMeta>();
    }
}
