using System;
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
        public CommandMetaList GetCommandMetaList(string nsp)
        {
            var modResource = main.ResourceManager.GetModResource(nsp);
            if (modResource == null)
                return null;
            return modResource.CommandMetaList;
        }
        public CommandMeta[] GetModCommandMetas(string nsp)
        {
            var metaList = GetCommandMetaList(nsp);
            if (metaList == null)
                return Array.Empty<CommandMeta>();
            return metaList.metas.ToArray();
        }
        public NamespaceID[] GetAllCommandsID()
        {
            return commandsCacheDict.Keys.ToArray();
        }
        #endregion

        #region 元数据
        public CommandMeta GetCommandMeta(NamespaceID commandID)
        {
            return commandsCacheDict.TryGetValue(commandID, out var meta) ? meta : null;
        }
        #endregion

        private Dictionary<NamespaceID, CommandMeta> commandsCacheDict = new Dictionary<NamespaceID, CommandMeta>();
    }
}
