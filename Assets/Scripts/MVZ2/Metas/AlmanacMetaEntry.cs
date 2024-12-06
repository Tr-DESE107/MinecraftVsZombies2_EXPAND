using System.Text;
using System.Xml;
using MVZ2.IO;
using MVZ2Logic;
using PVZEngine;

namespace MVZ2.Metas
{
    public class AlmanacMetaEntry
    {
        public NamespaceID id;
        public string name;
        public NamespaceID unlock;
        public SpriteReference sprite;
        public NamespaceID model;
        public string header;
        public string properties;
        public string flavor;
        public int index = -1;
        public static AlmanacMetaEntry FromXmlNode(XmlNode node, string defaultNsp)
        {
            var id = node.GetAttributeNamespaceID("id", defaultNsp);
            var name = node.GetAttribute("name");
            var unlock = node.GetAttributeNamespaceID("unlock", defaultNsp);
            var sprite = node.GetAttributeSpriteReference("sprite", defaultNsp);
            var model = node.GetAttributeNamespaceID("model", defaultNsp);
            var headerNode = node["header"];
            var propertiesNode = node["properties"];
            var flavorNode = node["flavor"];
            var header = headerNode != null ? ConcatNodeParagraphs(headerNode) : string.Empty;
            var properties = propertiesNode != null ? ConcatNodeParagraphs(propertiesNode) : string.Empty;
            var flavor = flavorNode != null ? ConcatNodeParagraphs(flavorNode) : string.Empty;
            return new AlmanacMetaEntry()
            {
                id = id,
                name = name,
                unlock = unlock,
                sprite = sprite,
                model = model,
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
            bool first = true;
            for (int i = 0; i < lineNodes.Count; i++)
            {
                var lineNode = lineNodes[i];
                if (lineNode.Name == "p")
                {
                    if (!first)
                    {
                        sb.Append("\n");
                    }
                    first = false;
                    sb.Append(lineNodes[i].InnerText);
                }
            }
            return sb.ToString();
        }
    }
}
