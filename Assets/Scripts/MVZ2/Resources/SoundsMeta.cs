using System.Xml;
using PVZEngine;
using UnityEngine;

namespace MVZ2
{
    public class SoundsMeta
    {
        public string root;
        public SoundResource[] resources;
        public static SoundsMeta FromXmlNode(string nsp, XmlNode node)
        {
            var sounds = new SoundResource[node.ChildNodes.Count];
            for (int i = 0; i < sounds.Length; i++)
            {
                sounds[i] = SoundResource.FromXmlNode(nsp, node.ChildNodes[i]);
            }
            return new SoundsMeta()
            {
                resources = sounds,
                root = node.GetAttribute("root"),
            };
        }
    }
}
