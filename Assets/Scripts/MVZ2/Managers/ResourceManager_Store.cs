using System.Collections.Generic;
using System.Linq;
using MVZ2.Metas;
using MVZ2.Modding;
using PVZEngine;
using UnityEngine;

namespace MVZ2.Managers
{
    public partial class ResourceManager : MonoBehaviour
    {
        #region 商品
        public ProductMeta GetProductMeta(NamespaceID groupID)
        {
            if (!NamespaceID.IsValid(groupID))
                return null;
            return productCacheDict.TryGetValue(groupID, out var meta) ? meta : null;
        }
        public bool HasProductMeta(NamespaceID groupID)
        {
            return GetProductMeta(groupID) != null;
        }
        public NamespaceID[] GetAllProductsID()
        {
            return productCacheDict.Keys.ToArray();
        }
        #endregion

        #region 闲聊
        public StoreChatMeta[] GetCharacterStoreChats(NamespaceID characterID)
        {
            if (!NamespaceID.IsValid(characterID))
                return null;
            if (!storeChats.TryGetValue(characterID, out var chats))
            {
                return null;
            }
            return chats.ToArray();
        }
        #endregion

        #region 预设
        public StorePresetMeta GetStorePresetMeta(NamespaceID presetID)
        {
            var modResource = GetModResource(presetID.SpaceName);
            if (modResource == null)
                return null;
            return modResource.StoreMetaList.Presets.FirstOrDefault(p => p.ID == presetID.Path);
        }
        public StorePresetMeta[] GetAllStorePresets()
        {
            return modResources.SelectMany(r => r.StoreMetaList.Presets).ToArray();
        }
        #endregion

        private void PostLoadMod_Store(string modNamespace, ModResource modResource)
        {
            foreach (var meta in modResource.StoreMetaList.Chats)
            {
                if (!storeChats.TryGetValue(meta.Character, out var list))
                {
                    list = new List<StoreChatMeta>();
                    storeChats.Add(meta.Character, list);
                }
                list.AddRange(meta.Chats);
            }
            foreach (var meta in modResource.StoreMetaList.Products)
            {
                if (meta.IsEmpty())
                    continue;
                productCacheDict.Add(new NamespaceID(modNamespace, meta.ID), meta);
            }
        }
        private void ClearResources_Store()
        {
            productCacheDict.Clear();
            storeChats.Clear();
        }

        private Dictionary<NamespaceID, ProductMeta> productCacheDict = new Dictionary<NamespaceID, ProductMeta>();
        private Dictionary<NamespaceID, List<StoreChatMeta>> storeChats = new Dictionary<NamespaceID, List<StoreChatMeta>>();
    }
}
