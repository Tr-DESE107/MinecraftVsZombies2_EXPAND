using System.Xml;
using PVZEngine;
using UnityEngine;

namespace MVZ2.Resources
{
    public class MapMeta
    {
        public string id;
        public NamespaceID model;
        public NamespaceID endlessUnlock;
        public Color backgroundColor;
        public NamespaceID[] stages;
        public static MapMeta FromXmlNode(XmlNode node, string defaultNsp)
        {
            var id = node.GetAttribute("id");
            var model = node.GetAttributeNamespaceID("model", defaultNsp);
            var endlessUnlock = node.GetAttributeNamespaceID("endlessUnlock", defaultNsp);
            var backgroundColor = node.GetAttributeColor("backgroundColor") ?? Color.black;

            var stagesNode = node["stages"];
            var stages = new NamespaceID[stagesNode.ChildNodes.Count];
            for (int i = 0; i < stages.Length; i++)
            {
                stages[i] = stagesNode.ChildNodes[i].GetAttributeNamespaceID("id", defaultNsp);
            }
            return new MapMeta()
            {
                id = id,
                model = model,
                endlessUnlock = endlessUnlock,
                backgroundColor = backgroundColor,
                stages = stages,
            };
        }
    }
}
