using System.Text;
using System.Xml;
using MVZ2.IO;
using PVZEngine;

namespace MVZ2.Metas
{
    public class AlmanacMetaEntry
    {
        public NamespaceID id;
        public string header;
        public string properties;
        public string flavor;
        public int index = -1;
        public static AlmanacMetaEntry FromXmlNode(XmlNode node, string defaultNsp)
        {
            var id = node.GetAttributeNamespaceID("id", defaultNsp);
            var headerNode = node["header"];
            var propertiesNode = node["properties"];
            var flavorNode = node["flavor"];
            var header = headerNode != null ? ConcatNodeParagraphs(headerNode) : string.Empty;
            var properties = propertiesNode != null ? ConcatNodeParagraphs(propertiesNode) : string.Empty;
            var flavor = flavorNode != null ? ConcatNodeParagraphs(flavorNode) : string.Empty;
            return new AlmanacMetaEntry()
            {
                id = id,
                header = header,
                properties = properties,
                flavor = flavor,
            };
        }
        public bool IsEmpty()
        {
            return !NamespaceID.IsValid(id);
        }
        private static string ConcatNodeParagraphs(XmlNode node)
        {
            var lineNodes = node.ChildNodes;
            var sb = new StringBuilder();
            for (int i = 0; i < lineNodes.Count; i++)
            {
                var lineNode = lineNodes[i];
                if (lineNode.Name == "p")
                {
                    sb.AppendLine(lineNodes[i].InnerText);
                }
            }
            return sb.ToString();
        }
    }
}
