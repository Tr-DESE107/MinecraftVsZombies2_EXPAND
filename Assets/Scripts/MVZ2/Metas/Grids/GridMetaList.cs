using System.Collections.Generic;
using System.Xml;

namespace MVZ2.Metas
{
    public class GridMetaList
    {
        public GridLayerMeta[] layers;
        public GridErrorMeta[] errors;
        public static GridMetaList FromXmlNode(XmlNode node, string defaultNsp)
        {
            var layersNode = node["layers"];
            var layers = new List<GridLayerMeta>();
            if (layersNode != null)
            {
                for (int i = 0; i < layersNode.ChildNodes.Count; i++)
                {
                    var child = layersNode.ChildNodes[i];
                    if (child.Name == "layer")
                    {
                        layers.Add(GridLayerMeta.FromXmlNode(child, defaultNsp));
                    }
                }
            }
            var errorsNode = node["errors"];
            var errors = new List<GridErrorMeta>();
            if (errorsNode != null)
            {
                for (int i = 0; i < errorsNode.ChildNodes.Count; i++)
                {
                    var child = errorsNode.ChildNodes[i];
                    if (child.Name == "error")
                    {
                        errors.Add(GridErrorMeta.FromXmlNode(child, defaultNsp));
                    }
                }
            }
            return new GridMetaList()
            {
                layers = layers.ToArray(),
                errors = errors.ToArray(),
            };
        }
    }
}
