using System.Xml;

namespace MVZ2.Metas
{
    public class FragmentMetaList
    {
        public FragmentMeta[] metas;
        public static FragmentMetaList FromXmlNode(XmlNode node)
        {
            var resources = new FragmentMeta[node.ChildNodes.Count];
            for (int i = 0; i < resources.Length; i++)
            {
                resources[i] = FragmentMeta.FromXmlNode(node.ChildNodes[i]);
            }
            return new FragmentMetaList()
            {
                metas = resources,
            };
        }
    }
}
