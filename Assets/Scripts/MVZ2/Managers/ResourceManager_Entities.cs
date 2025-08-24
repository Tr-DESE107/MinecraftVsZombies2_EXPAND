using System;
using System.Linq;
using MVZ2.Metas;
using MVZ2Logic;
using PVZEngine;
using UnityEngine;

namespace MVZ2.Managers
{
    public partial class ResourceManager : MonoBehaviour
    {
        #region 元数据列表
        public EntityMetaList GetEntityMetaList(string nsp)
        {
            var modResource = main.ResourceManager.GetModResource(nsp);
            if (modResource == null)
                return null;
            return modResource.EntityMetaList;
        }
        public EntityMeta[] GetModEntityMetas(string nsp)
        {
            var metaList = GetEntityMetaList(nsp);
            if (metaList == null)
                return Array.Empty<EntityMeta>();
            return metaList.metas.ToArray();
        }
        #endregion

        #region 元数据
        public string GetEntityDeathMessage(NamespaceID entityID)
        {
            return Main.Game.GetEntityDeathMessage(entityID);
        }
        public string GetEntityName(NamespaceID entityID)
        {
            return Main.Game.GetEntityName(entityID);
        }
        public string GetEntityTooltip(NamespaceID entityID)
        {
            return Main.Game.GetEntityTooltip(entityID);
        }
        #endregion

        #region 元数据
        public EntityCounterMeta GetEntityCounterMeta(NamespaceID counterID)
        {
            if (!NamespaceID.IsValid(counterID))
                return null;
            var modResource = GetModResource(counterID.SpaceName);
            if (modResource == null)
                return null;
            var list = modResource.EntityMetaList;
            if (list == null)
                return null;
            return list.counters.FirstOrDefault(m => m.ID == counterID.Path);
        }
        #endregion
    }
}
