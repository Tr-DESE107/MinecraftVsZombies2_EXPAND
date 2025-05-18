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
        public ArmorMetaList GetArmorMetaList(string nsp)
        {
            var modResource = main.ResourceManager.GetModResource(nsp);
            if (modResource == null)
                return null;
            return modResource.ArmorMetaList;
        }
        public ArmorMeta[] GetModArmorMetas(string nsp)
        {
            var metaList = GetArmorMetaList(nsp);
            if (metaList == null)
                return Array.Empty<ArmorMeta>();
            return metaList.metas.ToArray();
        }
        public NamespaceID[] GetAllArmorSlots()
        {
            return armorSlotsCacheDict.Keys.ToArray();
        }
        public NamespaceID[] GetAllArmorsID()
        {
            return armorsCacheDict.Keys.ToArray();
        }
        #endregion

        #region 元数据
        public ArmorSlotMeta GetArmorSlotMeta(NamespaceID armorID)
        {
            return armorSlotsCacheDict.TryGetValue(armorID, out var meta) ? meta : null;
        }
        public ArmorMeta GetArmorMeta(NamespaceID armorID)
        {
            return armorsCacheDict.TryGetValue(armorID, out var meta) ? meta : null;
        }
        #endregion

        private Dictionary<NamespaceID, ArmorMeta> armorsCacheDict = new Dictionary<NamespaceID, ArmorMeta>();
        private Dictionary<NamespaceID, ArmorSlotMeta> armorSlotsCacheDict = new Dictionary<NamespaceID, ArmorSlotMeta>();
    }
}
