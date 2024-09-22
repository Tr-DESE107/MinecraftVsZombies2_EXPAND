using System.Xml;
using PVZEngine;

namespace MVZ2.Resources
{
    public class AlmanacMetaEntry
    {
        public NamespaceID id;
        public string text;
        public static AlmanacMetaEntry FromXmlNode(XmlNode node, string defaultNsp)
        {
            var id = node.GetAttributeNamespaceID("id", defaultNsp);
            var lineNodes = node.ChildNodes;
            var lines = new string[lineNodes.Count];
            for (int i = 0; i < lines.Length; i++)
            {
                lines[i] = lineNodes[i].InnerText;
            }
            var text = string.Join("\n", lines);
            return new AlmanacMetaEntry()
            {
                id = id,
                text = text
            };
        }
        public bool IsEmpty()
        {
            return !NamespaceID.IsValid(id);
        }
    }
}
