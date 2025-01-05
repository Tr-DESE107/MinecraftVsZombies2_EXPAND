using System.Collections.Generic;
using System.Xml;

namespace MVZ2.Metas
{
    public class StoreMetaList
    {
        public StorePresetMeta[] Presets { get; private set; }
        public StoreChatGroupMeta[] Chats { get; private set; }
        public ProductMeta[] Products { get; private set; }
        public static StoreMetaList FromXmlNode(XmlNode node, string defaultNsp)
        {
            var presetsNode = node["presets"];
            var presets = new List<StorePresetMeta>();
            if (presetsNode != null)
            {
                for (var i = 0; i < presetsNode.ChildNodes.Count; i++)
                {
                    var child = presetsNode.ChildNodes[i];
                    if (child.Name == "preset")
                    {
                        presets.Add(StorePresetMeta.FromXmlNode(child, defaultNsp));
                    }
                }
            }
            var chatsNode = node["chats"];
            var chats = new List<StoreChatGroupMeta>();
            if (chatsNode != null)
            {
                for (var i = 0; i < chatsNode.ChildNodes.Count; i++)
                {
                    var child = chatsNode.ChildNodes[i];
                    if (child.Name == "group")
                    {
                        chats.Add(StoreChatGroupMeta.FromXmlNode(child, defaultNsp));
                    }
                }
            }
            var productsNode = node["products"];
            var products = new List<ProductMeta>();
            if (productsNode != null)
            {
                for (var i = 0; i < productsNode.ChildNodes.Count; i++)
                {
                    var child = productsNode.ChildNodes[i];
                    if (child.Name == "product")
                    {
                        products.Add(ProductMeta.FromXmlNode(child, defaultNsp, i));
                    }
                }
            }
            return new StoreMetaList()
            {
                Presets = presets.ToArray(),
                Chats = chats.ToArray(),
                Products = products.ToArray()
            };
        }
    }
}
