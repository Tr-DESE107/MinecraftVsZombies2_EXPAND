using System.Xml;
using UnityEngine;

namespace MVZ2Logic.Entities
{
    public class FragmentMeta
    {
        public string name;
        public Gradient gradient;
        public static FragmentMeta FromXmlNode(XmlNode node)
        {
            var name = node.GetAttribute("name");
            return new FragmentMeta()
            {
                name = name,
                gradient = node.ToGradient()
            };
        }
    }
}
