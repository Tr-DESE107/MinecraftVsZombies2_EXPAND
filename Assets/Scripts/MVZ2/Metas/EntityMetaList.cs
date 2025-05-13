using System.Collections.Generic;
using System.Xml;
using MVZ2.IO;
using PVZEngine;

namespace MVZ2.Metas
{
    public class EntityMetaList
    {
        public EntityMeta[] metas;
        public static EntityMetaList FromXmlNode(XmlNode node, string defaultNsp)
        {
            var templatesNode = node["templates"];
            var metaTemplates = EntityMetaTemplate.LoadTemplates(templatesNode, defaultNsp);

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
    public class EntityMetaTemplate
    {
        public string name;
        public int id;
        public List<NamespaceID> behaviours;
        public Dictionary<string, object> properties;
        private static EntityMetaTemplate LoadTemplate(XmlNode node, string defaultNsp, XmlNode rootNode)
        {
            var name = node.Name;
            var id = node.GetAttributeInt("id") ?? -1;
            var behaviours = new List<NamespaceID>();
            var properties = new Dictionary<string, object>();
            LoadTemplatePropertiesFromNode(node, defaultNsp, rootNode, behaviours, properties);

            return new EntityMetaTemplate()
            {
                name = name,
                id = id,
                behaviours = behaviours,
                properties = properties
            };
        }
        private static void LoadTemplatePropertiesFromNode(XmlNode node, string defaultNsp, XmlNode rootNode, List<NamespaceID> behaviours, Dictionary<string, object> properties)
        {
            var parent = node.GetAttribute("parent");
            if (!string.IsNullOrEmpty(parent))
            {
                var parentNode = rootNode[parent];
                if (parentNode != null)
                {
                    LoadTemplatePropertiesFromNode(parentNode, defaultNsp, rootNode, behaviours, properties);
                }
            }
            var behavioursNode = node["behaviours"];
            behavioursNode.ModifyEntityBehaviours(behaviours, defaultNsp);

            var propsNode = node["properties"];
            var props = propsNode.ToPropertyDictionary(defaultNsp);
            foreach (var prop in props)
            {
                properties[prop.Key] = prop.Value;
            }
        }
        public static EntityMetaTemplate[] LoadTemplates(XmlNode node, string defaultNsp)
        {
            List<EntityMetaTemplate> templates = new List<EntityMetaTemplate>();
            for (int i = 0; i < node.ChildNodes.Count; i++)
            {
                var templateNode = node.ChildNodes[i];
                var template = EntityMetaTemplate.LoadTemplate(templateNode, defaultNsp, node);
                templates.Add(template);
            }
            return templates.ToArray();
        }
    }
}
