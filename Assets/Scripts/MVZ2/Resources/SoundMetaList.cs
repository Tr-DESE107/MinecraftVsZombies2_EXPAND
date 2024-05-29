using System.Xml;
using PVZEngine;
using UnityEngine;

namespace MVZ2
{
    public class SoundMetaList
    {
        public SoundMeta[] metas;
        public static SoundMetaList FromXmlNode(XmlNode node)
        {
            var sounds = new SoundMeta[node.ChildNodes.Count];
            for (int i = 0; i < sounds.Length; i++)
            {
                sounds[i] = SoundMeta.FromXmlNode(node.ChildNodes[i]);
            }
            return new SoundMetaList()
            {
                metas = sounds,
            };
        }
    }
}
