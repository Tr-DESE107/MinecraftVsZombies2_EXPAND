using System.Xml;
using MVZ2.IO;
using PVZEngine;

namespace MVZ2.Metas
{
    public class MusicMeta
    {
        public string ID { get; private set; }
        public string Name { get; private set; }
        public NamespaceID Path { get; private set; }
        public static MusicMeta FromXmlNode(XmlNode node, string defaultNsp)
        {
            var id = node.GetAttribute("id");
            var name = node.GetAttribute("name");
            var path = node.GetAttributeNamespaceID("path", defaultNsp);
            return new MusicMeta()
            {
                ID = id,
                Name = name,
                Path = path
            };
        }
    }
}
