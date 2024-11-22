using System;
using System.Collections.Generic;
using System.Linq;
using MVZ2.Vanilla;
using MVZ2Logic;
using MVZ2Logic.Entities;
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
        public string GetEntityName(NamespaceID entityID)
        {
            if (entityID == null)
                return "null";
            var meta = GetEntityMeta(entityID);
            if (meta == null)
                return entityID.ToString();
            return Main.LanguageManager._p(Vanilla.VanillaStrings.CONTEXT_ENTITY_NAME, meta.name);
        }
        public string GetEntityDeathMessage(NamespaceID entityID)
        {
            string key = Vanilla.VanillaStrings.DEATH_MESSAGE_UNKNOWN;
            if (entityID != null)
            {
                var meta = GetEntityMeta(entityID);
                if (meta != null && meta.deathMessage != null)
                {
                    key = meta.deathMessage;
                }
            }
            return Main.LanguageManager._p(Vanilla.VanillaStrings.CONTEXT_DEATH_MESSAGE, key);
        }
        public string GetEntityTooltip(NamespaceID entityID)
        {
            if (entityID == null)
                return "null";
            var meta = GetEntityMeta(entityID);
            if (meta == null)
                return entityID.ToString();
            return Main.LanguageManager._p(Vanilla.VanillaStrings.CONTEXT_ENTITY_TOOLTIP, meta.tooltip);
        }
        #endregion

        private static readonly Dictionary<NamespaceID, EntityMeta> entitiesCacheDict = new Dictionary<NamespaceID, EntityMeta>();
    }
}
