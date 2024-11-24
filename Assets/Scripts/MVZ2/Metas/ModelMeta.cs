using System.Xml;
using MVZ2.IO;
using PVZEngine;

namespace MVZ2Logic.Models
{
    public class ModelMeta : IModelMeta
    {
        public string Name { get; private set; }
        public string Type { get; private set; }
        public NamespaceID Path { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public float XOffset { get; private set; }
        public float YOffset { get; private set; }
        public static ModelMeta FromXmlNode(XmlNode node, string defaultNsp)
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
                Name = name,
                Type = type,
                Path = NamespaceID.Parse(path, defaultNsp),
                Width = width,
                Height = height,
                XOffset = xOffset,
                YOffset = yOffset,
            };
        }
        public override string ToString()
        {
            return Name;
        }
    }
}
