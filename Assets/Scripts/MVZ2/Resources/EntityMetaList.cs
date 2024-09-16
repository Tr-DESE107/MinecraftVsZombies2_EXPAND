using System.Collections.Generic;
using System.Xml;

namespace MVZ2
{
    public class EntityMetaList
    {
        public EntityMeta[] metas;
        public static EntityMetaList FromXmlNode(XmlNode node)
        {
            var resources = new EntityMeta[node.ChildNodes.Count];
            for (int i = 0; i < resources.Length; i++)
            {
                var meta = EntityMeta.FromXmlNode(node.ChildNodes[i]);
                meta.order = i;
                resources[i] = meta;
            }
            return new EntityMetaList()
            {
                metas = resources,
            };
        }
    }
}
