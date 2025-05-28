﻿using System.Collections.Generic;
using System.Linq;
using MVZ2.Metas;
using PVZEngine;
using UnityEngine;

namespace MVZ2.Managers
{
    public partial class ResourceManager : MonoBehaviour
    {
        #region 元数据列表
        public ArcadeMetaList GetArcadeMetaList(string nsp)
        {
            var modResource = main.ResourceManager.GetModResource(nsp);
            if (modResource == null)
                return null;
            return modResource.ArcadeMetaList;
        }
        #endregion

        #region 元数据
        public ArcadeMeta GetArcadeMeta(NamespaceID arcade)
        {
            if (!NamespaceID.IsValid(arcade))
                return null;
            var modResource = main.ResourceManager.GetModResource(arcade.SpaceName);
            if (modResource == null)
                return null;
            return modResource.ArcadeMetaList.metas.FirstOrDefault(m => m.ID == arcade.Path);
        }
        #endregion
        public NamespaceID[] GetAllArcadeItems()
        {
            return arcadeCache.ToArray();
        }
        private List<NamespaceID> arcadeCache = new List<NamespaceID>();
    }
}
