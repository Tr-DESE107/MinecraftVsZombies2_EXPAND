using System.Xml;

namespace MVZ2Logic.Level
{
    public class AreaMetaList
    {
        public AreaMeta[] metas;
        public static AreaMetaList FromXmlNode(XmlNode node, string defaultNsp)
        {
            var resources = new AreaMeta[node.ChildNodes.Count];
            for (int i = 0; i < resources.Length; i++)
            {
                var meta = AreaMeta.FromXmlNode(node.ChildNodes[i], defaultNsp);
                resources[i] = meta;
            }
            return new AreaMetaList()
            {
                metas = resources,
            };
        }
    }
}
