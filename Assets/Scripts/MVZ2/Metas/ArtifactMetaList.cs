using System.Xml;

namespace MVZ2.Metas
{
    public class ArtifactMetaList
    {
        public ArtifactMeta[] metas;
        public static ArtifactMetaList FromXmlNode(XmlNode node, string defaultNsp)
        {
            var metas = new ArtifactMeta[node.ChildNodes.Count];
            for (int i = 0; i < metas.Length; i++)
            {
                var meta = ArtifactMeta.FromXmlNode(node.ChildNodes[i], defaultNsp, i);
                metas[i] = meta;
            }
            return new ArtifactMetaList()
            {
                metas = metas,
            };
        }
    }
}
