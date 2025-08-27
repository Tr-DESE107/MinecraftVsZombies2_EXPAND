using System.Collections.Generic;
using System.Xml;
using MVZ2.IO;
using PVZEngine;
using PVZEngine.Level.Collisions;

namespace MVZ2.Metas
{
    public class ArmorMeta
    {
        public string ID { get; private set; }
        public NamespaceID[] Behaviours { get; private set; }
        public ColliderConstructor[] ColliderConstructors { get; private set; }
        public Dictionary<string, object> Properties { get; private set; }
        public static ArmorMeta FromXmlNode(XmlNode node, string defaultNsp)
        {
            var id = node.GetAttribute("id");

            var behavioursNode = node["behaviours"];
            List<NamespaceID> behaviours = new List<NamespaceID>();
            if (behavioursNode != null)
            {
                for (int i = 0; i < behavioursNode.ChildNodes.Count; i++)
                {
                    var behaviourNode = behavioursNode.ChildNodes[i];
                    var behaviour = behaviourNode.GetAttributeNamespaceID("id", defaultNsp);
                    behaviours.Add(behaviour);
                }
            }

            var collidersNode = node["colliders"];
            List<ColliderConstructor> colliders = new List<ColliderConstructor>();
            if (collidersNode != null)
            {
                for (int i = 0; i < collidersNode.ChildNodes.Count; i++)
                {
                    var colliderNode = collidersNode.ChildNodes[i];
                    var collider = MetaXMLParser.LoadColliderConstructor(colliderNode);
                    colliders.Add(collider);
                }
            }

            var propsNode = node["props"];
            Dictionary<string, object> props = propsNode.ToPropertyDictionary(defaultNsp);
            Dictionary<string, object> properties = new Dictionary<string, object>();
            foreach (var prop in props)
            {
                var fullName = PropertyKeyHelper.ParsePropertyFullName(prop.Key, defaultNsp, PropertyRegions.armor);
                properties.Add(fullName, prop.Value);
            }

            return new ArmorMeta()
            {
                ID = id,
                Behaviours = behaviours.ToArray(),
                ColliderConstructors = colliders.ToArray(),
                Properties = properties
            };
        }
    }
}
