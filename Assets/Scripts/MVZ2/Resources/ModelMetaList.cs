using System.Xml;

namespace MVZ2
{
    public class ModelMetaList
    {
        public ModelMeta[] metas;
        public static ModelMetaList FromXmlNode(XmlNode node)
        {
            var metas = new ModelMeta[node.ChildNodes.Count];
            for (int i = 0; i < metas.Length; i++)
            {
                metas[i] = ModelMeta.FromXmlNode(node.ChildNodes[i]);
            }
            return new ModelMetaList()
            {
                metas = metas,
            };
        }
    }
}
