using System.Xml;
using PVZEngine;
using UnityEngine;

namespace MVZ2
{
    public class FragmentMetaList
    {
        public FragmentResource[] resources;
        public static FragmentMetaList FromXmlNode(XmlNode node)
        {
            var resources = new FragmentResource[node.ChildNodes.Count];
            for (int i = 0; i < resources.Length; i++)
            {
                resources[i] = FragmentResource.FromXmlNode(node.ChildNodes[i]);
            }
            return new FragmentMetaList()
            {
                resources = resources,
            };
        }
    }
}
