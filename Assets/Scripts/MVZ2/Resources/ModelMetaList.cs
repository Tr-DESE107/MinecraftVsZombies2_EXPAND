using System.Xml;
using PVZEngine;
using UnityEngine;

namespace MVZ2
{
    public class ModelMetaList
    {
        public ModelMeta[] metas;
        public static ModelMetaList FromXmlNode(XmlNode node)
        {
            var sounds = new ModelMeta[node.ChildNodes.Count];
            for (int i = 0; i < sounds.Length; i++)
            {
                sounds[i] = ModelMeta.FromXmlNode(node.ChildNodes[i]);
            }
            return new ModelMetaList()
            {
                metas = sounds,
            };
        }
    }
}
