using System.Xml;
using PVZEngine;
using UnityEngine;

namespace MVZ2
{
    public class ModelResource
    {
        public string name;
        public string type;
        public string path;
        public int width;
        public int height;
        public float xOffset;
        public float yOffset;
        public static ModelResource FromXmlNode(XmlNode node)
        {
            var name = node.GetAttribute("name");
            var type = node.GetAttribute("type");
            var path = node.GetAttribute("path");
            var width = node.GetAttributeInt("width") ?? 64;
            var height = node.GetAttributeInt("height") ?? 64;
            var xOffset = node.GetAttributeFloat("xOffset") ?? 0;
            var yOffset = node.GetAttributeFloat("yOffset") ?? 0;
            return new ModelResource()
            {
                name = name,
                type = type,
                path = path,
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
