using System.Collections.Generic;
using System.Xml;

namespace MVZ2.Metas
{
    public class ShapeMetaList
    {
        public ShapeMeta[] metas;
        public static ShapeMetaList FromXmlNode(XmlNode node, string defaultNsp)
        {
            var resources = new List<ShapeMeta>();
            for (int i = 0; i < node.ChildNodes.Count; i++)
            {
                var child = node.ChildNodes[i];
                resources.Add(ShapeMeta.FromXmlNode(child, defaultNsp));
            }
            return new ShapeMetaList()
            {
                metas = resources.ToArray(),
            };
        }
    }
}
