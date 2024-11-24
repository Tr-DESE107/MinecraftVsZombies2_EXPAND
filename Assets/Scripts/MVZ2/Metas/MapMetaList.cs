using System.Xml;

namespace MVZ2.Metas
{
    public class MapMetaList
    {
        public MapMeta[] metas;
        public static MapMetaList FromXmlNode(XmlNode node, string defaultNsp)
        {
            var resources = new MapMeta[node.ChildNodes.Count];
            for (int i = 0; i < resources.Length; i++)
            {
                var meta = MapMeta.FromXmlNode(node.ChildNodes[i], defaultNsp);
                resources[i] = meta;
            }
            return new MapMetaList()
            {
                metas = resources,
            };
        }
    }
}
