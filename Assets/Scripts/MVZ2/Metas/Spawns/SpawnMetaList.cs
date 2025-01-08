using System.Xml;

namespace MVZ2.Metas
{
    public class SpawnMetaList
    {
        public SpawnMeta[] Metas;
        public static SpawnMetaList FromXmlNode(XmlNode node, string defaultNsp)
        {
            var metas = new SpawnMeta[node.ChildNodes.Count];
            for (int i = 0; i < metas.Length; i++)
            {
                metas[i] = SpawnMeta.FromXmlNode(node.ChildNodes[i], defaultNsp);
            }
            return new SpawnMetaList()
            {
                Metas = metas,
            };
        }
    }
}
