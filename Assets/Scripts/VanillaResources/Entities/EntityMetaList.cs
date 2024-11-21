using System.Collections.Generic;
using System.Xml;

namespace MVZ2Logic.Entities
{
    public class EntityMetaList
    {
        public EntityMeta[] metas;
        public static EntityMetaList FromXmlNode(XmlNode node, string defaultNsp)
        {
            var templatesNode = node["templates"];
            var metaTemplates = GetTemplates(templatesNode, defaultNsp);

            var entriesNode = node["entries"];
            var resources = new EntityMeta[entriesNode.ChildNodes.Count];
            for (int i = 0; i < resources.Length; i++)
            {
                var meta = EntityMeta.FromXmlNode(entriesNode.ChildNodes[i], defaultNsp, metaTemplates);
                meta.order = i;
                resources[i] = meta;
            }
            return new EntityMetaList()
            {
                metas = resources,
            };
        }
        public static EntityMetaTemplate[] GetTemplates(XmlNode node, string defaultNsp)
        {
            List<EntityMetaTemplate> templates = new List<EntityMetaTemplate>();
            for (int i = 0; i < node.ChildNodes.Count; i++)
            {
                var templateNode = node.ChildNodes[i];
                var template = LoadTemplate(templateNode, defaultNsp, node);
                templates.Add(template);
            }
            return templates.ToArray();
        }
        private static EntityMetaTemplate LoadTemplate(XmlNode node, string defaultNsp, XmlNode rootNode)
        {
            var name = node.Name;
            var id = node.GetAttributeInt("id") ?? -1;
            var properties = new Dictionary<string, object>();
            LoadTemplatePropertiesFromNode(node, defaultNsp, rootNode, properties);

            return new EntityMetaTemplate()
            {
                name = name,
                id = id,
                properties = properties
            };
        }
        private static void LoadTemplatePropertiesFromNode(XmlNode node, string defaultNsp, XmlNode rootNode, Dictionary<string, object> target)
        {
            var parent = node.GetAttribute("parent");
            if (!string.IsNullOrEmpty(parent))
            {
                var parentNode = rootNode[parent];
                if (parentNode != null)
                {
                    LoadTemplatePropertiesFromNode(parentNode, defaultNsp, rootNode, target);
                }
            }
            var props = node.ToPropertyDictionary(defaultNsp);
            foreach (var prop in props)
            {
                target[prop.Key] = prop.Value;
            }
        }
    }
    public class EntityMetaTemplate
    {
        public string name;
        public int id;
        public Dictionary<string, object> properties;
    }
}
