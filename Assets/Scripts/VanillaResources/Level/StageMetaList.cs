using System.Xml;

namespace MVZ2.Resources
{
    public class StageMetaList
    {
        public StageMeta[] metas;
        public static StageMetaList FromXmlNode(XmlNode node, string defaultNsp)
        {
            var resources = new StageMeta[node.ChildNodes.Count];
            for (int i = 0; i < resources.Length; i++)
            {
                var meta = StageMeta.FromXmlNode(node.ChildNodes[i], defaultNsp);
                resources[i] = meta;
            }
            return new StageMetaList()
            {
                metas = resources,
            };
        }
    }
}
