using System.Xml;
using PVZEngine;
using UnityEngine;

namespace MVZ2
{
    public class ModelsMeta
    {
        public string root;
        public ModelResource[] resources;
        public static ModelsMeta FromXmlNode(XmlNode node)
        {
            var sounds = new ModelResource[node.ChildNodes.Count];
            for (int i = 0; i < sounds.Length; i++)
            {
                sounds[i] = ModelResource.FromXmlNode(node.ChildNodes[i]);
            }
            return new ModelsMeta()
            {
                resources = sounds,
                root = node.GetAttribute("root"),
            };
        }
    }
}
