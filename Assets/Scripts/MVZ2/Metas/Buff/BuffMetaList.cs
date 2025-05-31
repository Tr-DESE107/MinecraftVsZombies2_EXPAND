using System.Xml;

namespace MVZ2.Metas
{
    public class BuffMetaList
    {
        public BuffMeta[] metas;
        public static BuffMetaList FromXmlNode(XmlNode node, string defaultNsp)
        {
            var metas = new BuffMeta[node.ChildNodes.Count];
            for (int i = 0; i < metas.Length; i++)
            {
                metas[i] = BuffMeta.FromXmlNode(node.ChildNodes[i], defaultNsp);
            }
            return new BuffMetaList()
            {
                metas = metas,
            };
        }
    }
}
