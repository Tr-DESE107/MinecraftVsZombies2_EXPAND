using System.Xml;
using MVZ2.IO;
using MVZ2Logic;
using MVZ2Logic.SeedPacks;

namespace MVZ2.Metas
{
    public class BlueprintOptionMeta : ISeedOptionMeta
    {
        public string ID { get; private set; }
        public int Cost { get; private set; }
        public string Name { get; private set; }
        public SpriteReference Icon { get; private set; }
        public static BlueprintOptionMeta FromXmlNode(XmlNode node, string defaultNsp)
        {
            var id = node.GetAttribute("id");
            var cost = node.GetAttributeInt("cost") ?? 0;
            var name = node.GetAttribute("name");
            var icon = node.GetAttributeSpriteReference("icon", defaultNsp);
            return new BlueprintOptionMeta()
            {
                ID = id,
                Cost = cost,
                Name = name,
                Icon = icon
            };
        }
    }
}
