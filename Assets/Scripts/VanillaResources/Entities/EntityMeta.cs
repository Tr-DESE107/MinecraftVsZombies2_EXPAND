using System.Collections.Generic;
using System.Linq;
using System.Xml;
using PVZEngine;
using PVZEngine.Entities;

namespace MVZ2Logic.Entities
{
    public class EntityMeta
    {
        public int type;
        public string id;
        public string name;
        public string deathMessage;
        public string tooltip;
        public NamespaceID unlock;
        public int order;
        public Dictionary<string, object> properties;
        public static EntityMeta FromXmlNode(XmlNode node, string defaultNsp, IEnumerable<EntityMetaTemplate> templates)
        {
            var type = EntityTypes.EFFECT;
            var template = templates.FirstOrDefault(t => t.name == node.Name);
            var id = node.GetAttribute("id");
            var name = node.GetAttribute("name");
            var deathMessage = node.GetAttribute("deathMessage");
            var unlock = node.GetAttributeNamespaceID("unlock", defaultNsp);
            var tooltip = node.GetAttribute("tooltip");

            Dictionary<string, object> properties = node.ToPropertyDictionary(defaultNsp);
            if (template != null)
            {
                type = template.id;
                foreach (var prop in template.properties)
                {
                    if (properties.ContainsKey(prop.Key))
                        continue;
                    properties.Add(prop.Key, prop.Value);
                }
            }
            return new EntityMeta()
            {
                type = type,
                id = id,
                name = name,
                deathMessage = deathMessage,
                tooltip = tooltip,
                unlock = unlock,
                properties = properties,
            };
        }
    }
}
