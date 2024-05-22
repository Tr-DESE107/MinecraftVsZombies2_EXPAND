using System.Xml;
using PVZEngine;
using UnityEngine;

namespace MVZ2
{
    public class ModelResource
    {
        public NamespaceID id;
        public string path;
        public int width;
        public int height;
        public float xOffset;
        public float yOffset;
        public static ModelResource FromXmlNode(string nsp, XmlNode node)
        {
            var name = node.GetAttribute("name");
            var path = node.GetAttribute("path");
            var width = node.GetAttributeInt("width") ?? 64;
            var height = node.GetAttributeInt("height") ?? 64;
            var xOffset = node.GetAttributeFloat("xOffset") ?? 0;
            var yOffset = node.GetAttributeFloat("yOffset") ?? 0;
            return new ModelResource()
            {
                id = new NamespaceID(nsp, name),
                path = path,
                width = width,
                height = height,
                xOffset = xOffset,
                yOffset = yOffset,
            };
        }
    }
}
