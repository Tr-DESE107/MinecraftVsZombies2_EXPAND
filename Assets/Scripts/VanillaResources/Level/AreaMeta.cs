using System.Xml;
using PVZEngine;

namespace MVZ2Logic.Level
{
    public class AreaMeta
    {
        public string id;
        public NamespaceID model;
        public NamespaceID music;
        public static AreaMeta FromXmlNode(XmlNode node, string defaultNsp)
        {
            var id = node.GetAttribute("id");
            var model = node.GetAttributeNamespaceID("model", defaultNsp);
            var music = node.GetAttributeNamespaceID("music", defaultNsp);
            return new AreaMeta()
            {
                id = id,
                model = model,
                music = music,
            };
        }
    }
}
