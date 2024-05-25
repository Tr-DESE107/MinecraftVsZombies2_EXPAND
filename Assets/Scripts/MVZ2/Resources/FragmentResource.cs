using System.Xml;
using PVZEngine;
using UnityEngine;

namespace MVZ2
{
    public class FragmentResource
    {
        public string name;
        public Gradient gradient;
        public static FragmentResource FromXmlNode(XmlNode node)
        {
            var name = node.GetAttribute("name");
            return new FragmentResource()
            {
                name = name,
                gradient = node.ToGradient()
            };
        }
    }
}
