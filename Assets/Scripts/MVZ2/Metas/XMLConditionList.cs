using System.Collections.Generic;
using System.Xml;

namespace MVZ2.Metas
{
    public class XMLConditionList
    {
        public XMLCondition[] Conditions { get; private set; }
        public static XMLConditionList FromXmlNode(XmlNode node, string defaultNsp)
        {
            if (node == null)
                return null;
            List<XMLCondition> conditions = new List<XMLCondition>();
            for (int i = 0; i < node.ChildNodes.Count; i++)
            {
                var childNode = node.ChildNodes[i];
                if (childNode.Name == "condition")
                {
                    conditions.Add(XMLCondition.FromXmlNode(childNode, defaultNsp));
                }
            }
            return new XMLConditionList()
            {
                Conditions = conditions.ToArray()
            };
        }
    }
}
