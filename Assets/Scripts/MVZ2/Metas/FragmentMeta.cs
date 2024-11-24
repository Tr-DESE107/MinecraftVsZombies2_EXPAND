using System.Xml;
using MVZ2.IO;
using MVZ2Logic.Fragments;
using UnityEngine;

namespace MVZ2.Metas
{
    public class FragmentMeta : IFragmentMeta
    {
        public string Name { get; private set; }
        public Gradient Gradient { get; private set; }
        public static FragmentMeta FromXmlNode(XmlNode node)
        {
            var name = node.GetAttribute("name");
            return new FragmentMeta()
            {
                Name = name,
                Gradient = node.ToGradient()
            };
        }
    }
}
