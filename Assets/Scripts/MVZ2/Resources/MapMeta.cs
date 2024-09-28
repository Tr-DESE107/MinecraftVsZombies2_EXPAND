using UnityEngine;
using System.Xml;
using MVZ2.GameContent;
using PVZEngine;
using PVZEngine.Definitions;
using PVZEngine.Level;

namespace MVZ2.Resources
{
    public class MapMeta
    {
        public string id;
        public NamespaceID path;
        public Color backgroundColor;
        public static MapMeta FromXmlNode(XmlNode node, string defaultNsp)
        {
            var id = node.GetAttribute("id");
            var path = node.GetAttributeNamespaceID("path", defaultNsp);
            var backgroundColor = node.GetAttributeColor("backgroundColor") ?? Color.black;
            return new MapMeta()
            {
                id = id,
                path = path,
                backgroundColor = backgroundColor,
            };
        }
    }
}
