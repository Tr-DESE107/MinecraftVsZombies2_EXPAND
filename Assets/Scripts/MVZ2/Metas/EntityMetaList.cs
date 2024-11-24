using System.Xml;
using MVZ2Logic.Entities;

namespace MVZ2.Metas
{
    public class EntityMetaList
    {
        public EntityMeta[] metas;
        public static EntityMetaList FromXmlNode(XmlNode node, string defaultNsp)
        {
            var templatesNode = node["templates"];
            var metaTemplates = templatesNode.LoadEntityTemplates(defaultNsp);

            var entriesNode = node["entries"];
            var resources = new EntityMeta[entriesNode.ChildNodes.Count];
            for (int i = 0; i < resources.Length; i++)
            {
                var meta = EntityMeta.FromXmlNode(entriesNode.ChildNodes[i], defaultNsp, metaTemplates, i);
                resources[i] = meta;
            }
            return new EntityMetaList()
            {
                metas = resources,
            };
        }
    }
}
