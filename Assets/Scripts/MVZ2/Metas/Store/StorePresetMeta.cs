using System.Xml;
using MVZ2.IO;
using MVZ2Logic;
using PVZEngine;

namespace MVZ2.Metas
{
    public class StorePresetMeta
    {
        public string ID { get; private set; }
        public NamespaceID Character { get; private set; }
        public SpriteReference Background { get; private set; }
        public NamespaceID Music { get; private set; }
        public int Priority { get; private set; }
        public XMLConditionList Conditions { get; private set; }
        public static StorePresetMeta FromXmlNode(XmlNode node, string defaultNsp)
        {
            var id = node.GetAttribute("id");
            var character = node.GetAttributeNamespaceID("character", defaultNsp);
            var background = node.GetAttributeSpriteReference("background", defaultNsp);
            var music = node.GetAttributeNamespaceID("music", defaultNsp);
            var priority = node.GetAttributeInt("priority") ?? 0;

            XMLConditionList conditions = null;
            var conditionsNode = node["conditions"];
            if (conditionsNode != null)
            {
                conditions = XMLConditionList.FromXmlNode(conditionsNode, defaultNsp);
            }
            return new StorePresetMeta()
            {
                ID = id,
                Character = character,
                Background = background,
                Music = music,
                Priority = priority,
                Conditions = conditions
            };
        }
    }
}
