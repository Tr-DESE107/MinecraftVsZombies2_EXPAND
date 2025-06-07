﻿using System.Collections.Generic;
using System.Linq;
using MVZ2.Metas;
using MVZ2.Vanilla.Almanacs;
using PVZEngine;
using UnityEngine;

namespace MVZ2.Managers
{
    public partial class ResourceManager : MonoBehaviour
    {
        public AlmanacMetaList GetAlmanacMetaList(string nsp)
        {
            var modResource = GetModResource(nsp);
            if (modResource == null)
                return null;
            return modResource.AlmanacMetaList;
        }
        public AlmanacMetaEntry GetAlmanacMetaEntry(string type, NamespaceID id)
        {
            if (!NamespaceID.IsValid(id))
                return null;
            var metaList = GetAlmanacMetaList(id.SpaceName);
            if (metaList == null)
                return null;
            if (!metaList.TryGetCategory(type, out var entries))
                return null;
            var entry = entries.entries.FirstOrDefault(e => e.id == id);
            if (entry != null)
                return entry;
            return entries.groups.SelectMany(g => g.entries).FirstOrDefault(e => e.id == id);
        }
        public AlmanacTagMeta GetAlmanacTagMeta(NamespaceID id)
        {
            if (!NamespaceID.IsValid(id))
                return null;
            var metaList = GetAlmanacMetaList(id.SpaceName);
            if (metaList == null)
                return null;
            return metaList.tags.FirstOrDefault(e => e.id == id.Path);
        }
        public AlmanacTagEnumMeta GetAlmanacTagEnumMeta(NamespaceID id)
        {
            if (!NamespaceID.IsValid(id))
                return null;
            var metaList = GetAlmanacMetaList(id.SpaceName);
            if (metaList == null)
                return null;
            return metaList.enums.FirstOrDefault(e => e.id == id.Path);
        }
        public IEnumerable<AlmanacTagMeta> GetAllAlmanacTagMetas()
        {
            foreach (var modResource in modResources)
            {
                var metaList = modResource.AlmanacMetaList;
                if (metaList == null)
                    continue;
                foreach (var tag in metaList.tags)
                {
                    yield return tag;
                }
            }
        }
        public IEnumerable<AlmanacTagEnumMeta> GetAllAlmanacTagEnumMetas()
        {
            foreach (var modResource in modResources)
            {
                var metaList = modResource.AlmanacMetaList;
                if (metaList == null)
                    continue;
                foreach (var tagEnum in metaList.enums)
                {
                    yield return tagEnum;
                }
            }
        }
        public bool IsContraptionInAlmanac(NamespaceID id)
        {
            var entry = GetAlmanacMetaEntry(VanillaAlmanacCategories.CONTRAPTIONS, id);
            return entry != null && entry.index >= 0;
        }
        public bool IsEnemyInAlmanac(NamespaceID id)
        {
            var entry = GetAlmanacMetaEntry(VanillaAlmanacCategories.ENEMIES, id);
            return entry != null && entry.index >= 0;
        }
    }
}
