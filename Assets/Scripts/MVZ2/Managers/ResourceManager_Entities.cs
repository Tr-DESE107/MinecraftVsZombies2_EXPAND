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
        public NamespaceID[] GetAllEntitiesID()
        {
            return entitiesCacheDict.Keys.ToArray();
        }
        #endregion

        #region 元数据
        public EntityMeta GetEntityMeta(NamespaceID entityID)
        {
            return entitiesCacheDict.TryGetValue(entityID, out var meta) ? meta : null;
        }
        public string GetEntityDeathMessage(NamespaceID entityID)
        {
            string key = VanillaStrings.DEATH_MESSAGE_UNKNOWN;
            if (entityID != null)
            {
                var meta = GetEntityMeta(entityID);
                if (meta != null && meta.DeathMessage != null)
                {
                    key = meta.DeathMessage;
                }
            }
            return Main.LanguageManager._p(VanillaStrings.CONTEXT_DEATH_MESSAGE, key);
        }
        public string GetEntityName(NamespaceID entityID)
        {
            return Main.Game.GetEntityName(entityID);
        }
        public string GetEntityTooltip(NamespaceID entityID)
        {
            if (entityID == null)
                return "null";
            var meta = GetEntityMeta(entityID);
            if (meta == null)
                return entityID.ToString();
            var tooltip = meta.Tooltip ?? VanillaStrings.UNKNOWN_ENTITY_TOOLTIP;
            return Main.LanguageManager._p(VanillaStrings.CONTEXT_ENTITY_TOOLTIP, tooltip);
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

        private Dictionary<NamespaceID, EntityMeta> entitiesCacheDict = new Dictionary<NamespaceID, EntityMeta>();
    }
}
