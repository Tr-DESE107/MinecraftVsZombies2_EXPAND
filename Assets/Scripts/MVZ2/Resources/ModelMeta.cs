using System.Xml;
using PVZEngine;

namespace MVZ2
{
    public class ModelMeta
    {
        public string name;
        public string type;
        public NamespaceID path;
        public int width;
        public int height;
        public float xOffset;
        public float yOffset;
        public static ModelMeta FromXmlNode(XmlNode node)
        {
            var name = node.GetAttribute("name");
            var type = node.GetAttribute("type");
            var path = node.GetAttribute("path");
            var width = node.GetAttributeInt("width") ?? 64;
            var height = node.GetAttributeInt("height") ?? 64;
            var xOffset = node.GetAttributeFloat("xOffset") ?? 0;
            var yOffset = node.GetAttributeFloat("yOffset") ?? 0;
            return new ModelMeta()
            {
                name = name,
                type = type,
                path = ResourceManager.ParseNamespaceID(path),
                width = width,
                height = height,
                xOffset = xOffset,
                yOffset = yOffset,
            };
        }
        public override string ToString()
        {
            return name;
        }
    }
}
