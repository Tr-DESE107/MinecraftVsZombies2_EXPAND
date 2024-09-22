using System.Xml;

namespace MVZ2.Resources
{
    public class EntityMetaList
    {
        public EntityMeta[] metas;
        public static EntityMetaList FromXmlNode(XmlNode node, string defaultNsp)
        {
            var resources = new EntityMeta[node.ChildNodes.Count];
            for (int i = 0; i < resources.Length; i++)
            {
                var meta = EntityMeta.FromXmlNode(node.ChildNodes[i], defaultNsp);
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
