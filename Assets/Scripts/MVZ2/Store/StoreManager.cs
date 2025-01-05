using System;
using System.Collections.Generic;
using System.Linq;
using MukioI18n;
using MVZ2.Managers;
using MVZ2.Metas;
using MVZ2.Saves;
using MVZ2.Store;
using PVZEngine;
using Tools;
using UnityEngine;

namespace MVZ2.Almanacs
{
    public class StoreManager : MonoBehaviour
    {
        public void GetOrderedProducts(IEnumerable<NamespaceID> products, int countPerRow, List<NamespaceID> appendList)
        {
            var idList = GetIDListByProductOrder(products);
            var ordered = CompressLayout(idList, countPerRow);
            appendList.AddRange(ordered);
        }
        public StoreChatMeta GetRandomChat(NamespaceID characterId, RandomGenerator rng)
        {
            var characterChats = Main.ResourceManager.GetCharacterStoreChats(characterId);
            if (characterChats == null)
                return null;
            return characterChats.Random(rng);
        }
        public ProductItemViewData GetProductViewData(NamespaceID id)
        {
            if (!NamespaceID.IsValid(id))
                return ProductItemViewData.Empty;
            var productMeta = Main.ResourceManager.GetProductMeta(id);
            if (productMeta == null)
                return ProductItemViewData.Empty;

            bool isBlueprint = NamespaceID.IsValid(productMeta.BlueprintID);
            var icon = Main.GetFinalSprite(productMeta.Sprite);
            var blueprint = Main.ResourceManager.GetBlueprintViewData(productMeta.BlueprintID);

            string price = string.Empty;
            bool interactable = false;
            string text = string.Empty;

            var stage = GetCurrentProductStage(id);
            if (stage != null)
            {
                bool soldout = Main.SaveManager.IsUnlocked(stage.Unlocks);
                price = string.Format("{0:#,#}", stage.Price);
                interactable = !soldout;
                var textKey = soldout ? PRODUCT_SOLDOUT : stage.Text;
                if (!string.IsNullOrEmpty(textKey))
                    text = Main.LanguageManager._(textKey);
            }

            return new ProductItemViewData()
            {
                icon = icon,
                blueprint = blueprint,
                isBlueprint = isBlueprint,
                isBlueprintMobile = Main.IsMobile(),

                interactable = interactable,
                price = price,
                text = text
            };
        }
        public ProductStageMeta GetCurrentProductStage(NamespaceID productId)
        {
            var productMeta = Main.ResourceManager.GetProductMeta(productId);
            if (productMeta == null)
                return null;
            for (int i = 0; i < productMeta.Stages.Length; i++)
            {
                var stage = productMeta.Stages[i];
                if (stage == null)
                    continue;
                if (stage.Conditions != null && !Main.SaveManager.MeetsXMLConditions(stage.Conditions))
                    continue;
                if (Main.SaveManager.IsUnlocked(stage.Unlocks))
                    continue;
                return stage;
            }
            return productMeta.Stages.LastOrDefault();
        }
        private NamespaceID[] GetIDListByProductOrder(IEnumerable<NamespaceID> idList)
        {
            if (idList == null || idList.Count() == 0)
                return Array.Empty<NamespaceID>();
            var productIndexes = idList.Select(id => (id, index: Main.ResourceManager.GetProductMeta(id)?.Index ?? -1));
            var maxIndex = productIndexes.Max(tuple => tuple.index);
            var ordered = new NamespaceID[maxIndex + 1];
            for (int i = 0; i < ordered.Length; i++)
            {
                var tuple = productIndexes.FirstOrDefault(tuple => tuple.index == i);
                ordered[i] = tuple.id;
            }
            return ordered;
        }
        private IEnumerable<NamespaceID> CompressLayout(IEnumerable<NamespaceID> idList, int countPerRow)
        {
            var groups = idList
                .Select((v, i) => (v, i))
                .GroupBy(p => p.i / countPerRow);
            return groups
                .Where(g => !g.All(p => !NamespaceID.IsValid(p.v)))
                .SelectMany(g => g.Select(p => p.v))
                .ToArray();
        }
        [TranslateMsg("商店文本")]
        public const string PRODUCT_SOLDOUT = "售罄";
        public MainManager Main => MainManager.Instance;
    }
}
