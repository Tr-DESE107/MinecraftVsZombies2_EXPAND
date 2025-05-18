using System.Collections.Generic;
using System.Linq;
using System.Xml;
using MVZ2.IO;
using MVZ2Logic.Entities;
using PVZEngine;
using PVZEngine.Entities;

namespace MVZ2.Metas
{
    public class EntityMeta : IEntityMeta
    {
        public int Type { get; private set; }
        public string ID { get; private set; }
        public string Name { get; private set; }
        public string DeathMessage { get; private set; }
        public string Tooltip { get; private set; }
        public NamespaceID Unlock { get; private set; }
        public int Order { get; private set; }
        public NamespaceID[] Behaviours { get; private set; }
        public Dictionary<string, object> Properties { get; private set; }
        public static EntityMeta FromXmlNode(XmlNode node, string defaultNsp, IEnumerable<EntityMetaTemplate> templates, int order)
        {
            var type = EntityTypes.EFFECT;
            var template = templates.FirstOrDefault(t => t.name == node.Name);
            var id = node.GetAttribute("id");
            var name = node.GetAttribute("name");
            var deathMessage = node.GetAttribute("deathMessage")?.Replace("\\n", "\n");
            var unlock = node.GetAttributeNamespaceID("unlock", defaultNsp);
            var tooltip = node.GetAttribute("tooltip")?.Replace("\\n", "\n");

            var behaviours = new List<NamespaceID>();
            var behavioursNode = node["behaviours"];
            var propertyNode = node["properties"];
            Dictionary<string, object> properties = propertyNode.ToPropertyDictionary(defaultNsp);
            if (template != null)
            {
                type = template.id;

                behaviours.AddRange(template.behaviours);
                behavioursNode.ModifyEntityBehaviours(behaviours, defaultNsp);

                foreach (var prop in template.properties)
                {
                    if (properties.ContainsKey(prop.Key))
                        continue;
                    properties.Add(prop.Key, prop.Value);
                }
            }
            return new EntityMeta()
            {
                Type = type,
                ID = id,
                Name = name,
                DeathMessage = deathMessage,
                Tooltip = tooltip,
                Unlock = unlock,
                Behaviours = behaviours.ToArray(),
                Properties = properties,
                Order = order,
            };
        }
    }

    public class EntityBehaviourItem
    {
        public BehaviourOperator Operator { get; private set; }
        public NamespaceID ID { get; private set; }
        public static EntityBehaviourItem FromXmlNode(XmlNode node, string defaultNsp)
        {
            var opStr = node.GetAttribute("operator");
            var op = BehaviourOperator.Add;
            if (!string.IsNullOrEmpty(opStr) && operatorDict.TryGetValue(opStr, out var o))
            {
                op = o;
            }
            var id = node.GetAttributeNamespaceID("id", defaultNsp);
            return new EntityBehaviourItem()
            {
                Operator = op,
                ID = id
            };
        }
        private static Dictionary<string, BehaviourOperator> operatorDict = new Dictionary<string, BehaviourOperator>()
        {
            { "add", BehaviourOperator.Add },
            { "remove", BehaviourOperator.Remove },
        };
    }
    public enum BehaviourOperator
    {
        Add,
        Remove
    }
}
