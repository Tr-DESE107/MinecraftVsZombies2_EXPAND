using System.Xml;
using MVZ2.IO;
using MVZ2Logic;
using MVZ2Logic.SeedPacks;
using PVZEngine;

namespace MVZ2.Metas
{
    public class BlueprintOptionMeta : ISeedOptionMeta
    {
        public string ID { get; private set; }
        public int Cost { get; private set; }
        public string Name { get; private set; }
        public BlueprintMetaIcon Icon { get; private set; }
        public static BlueprintOptionMeta FromXmlNode(XmlNode node, string defaultNsp)
        {
            var id = node.GetAttribute("id");
            var cost = node.GetAttributeInt("cost") ?? 0;
            var name = node.GetAttribute("name");
            BlueprintMetaIcon icon = null;
            var iconNode = node["icon"];
            if (iconNode != null)
            {
                icon = BlueprintMetaIcon.FromXmlNode(iconNode, defaultNsp);
            }
            return new BlueprintOptionMeta()
            {
                ID = id,
                Cost = cost,
                Name = name,
                Icon = icon
            };
        }
        public SpriteReference GetIcon()
        {
            return Icon?.Sprite;
        }
        public NamespaceID GetModelID()
        {
            return Icon?.ModelID;
        }
    }
}
