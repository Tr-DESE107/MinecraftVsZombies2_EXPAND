using System.Xml;
using PVZEngine;
using UnityEngine;

namespace MVZ2
{
    public class SoundsMeta
    {
        public string root;
        public SoundResource[] resources;
        public static SoundsMeta FromXmlNode(XmlNode node)
        {
            var sounds = new SoundResource[node.ChildNodes.Count];
            for (int i = 0; i < sounds.Length; i++)
            {
                sounds[i] = SoundResource.FromXmlNode(node.ChildNodes[i]);
            }
            return new SoundsMeta()
            {
                resources = sounds,
                root = node.GetAttribute("root"),
            };
        }
    }
}
