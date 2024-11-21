using System.Xml;

namespace MVZ2.Resources
{
    public class DifficultyMetaList
    {
        public DifficultyMeta[] metas;
        public static DifficultyMetaList FromXmlNode(XmlNode node)
        {
            var resources = new DifficultyMeta[node.ChildNodes.Count];
            for (int i = 0; i < resources.Length; i++)
            {
                resources[i] = DifficultyMeta.FromXmlNode(node.ChildNodes[i]);
            }
            return new DifficultyMetaList()
            {
                metas = resources,
            };
        }
    }
}
