using System.Xml;
using MVZ2Logic.Models;

namespace MVZ2.Metas
{
    public class ModelMetaList
    {
        public ModelMeta[] metas;
        public static ModelMetaList FromXmlNode(XmlNode node, string defaultNsp)
        {
            var metas = new ModelMeta[node.ChildNodes.Count];
            for (int i = 0; i < metas.Length; i++)
            {
                metas[i] = ModelMeta.FromXmlNode(node.ChildNodes[i], defaultNsp);
            }
            return new ModelMetaList()
            {
                metas = metas,
            };
        }
    }
}
