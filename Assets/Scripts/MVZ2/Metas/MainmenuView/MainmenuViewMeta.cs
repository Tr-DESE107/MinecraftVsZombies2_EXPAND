using System.Xml;
using MVZ2.IO;
using PVZEngine;

namespace MVZ2.Metas
{
    public class MainmenuViewMeta
    {
        public string ID { get; private set; }
        public int Priority { get; private set; }
        public NamespaceID SpritesheetID { get; private set; }
        public XMLConditionList Conditions { get; private set; }
        public static MainmenuViewMeta FromXmlNode(XmlNode node, string defaultNsp)
        {
            var id = node.GetAttribute("id");
            var priority = node.GetAttributeInt("priority") ?? 0;
            var spritesheet = node.GetAttributeNamespaceID("spritesheet", defaultNsp);
            var conditionsNode = node["conditions"];
            XMLConditionList conditions = null;
            if (conditionsNode != null)
            {
                conditions = XMLConditionList.FromXmlNode(conditionsNode, defaultNsp);
            }
            return new MainmenuViewMeta()
            {
                ID = id,
                Priority = priority,
                SpritesheetID = spritesheet,
                Conditions = conditions
            };
        }
    }
}
