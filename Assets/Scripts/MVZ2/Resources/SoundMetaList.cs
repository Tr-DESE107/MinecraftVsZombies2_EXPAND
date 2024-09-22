using System.Xml;

namespace MVZ2.Resources
{
    public class SoundMetaList
    {
        public SoundMeta[] metas;
        public static SoundMetaList FromXmlNode(XmlNode node, string defaultNsp)
        {
            var sounds = new SoundMeta[node.ChildNodes.Count];
            for (int i = 0; i < sounds.Length; i++)
            {
                sounds[i] = SoundMeta.FromXmlNode(node.ChildNodes[i], defaultNsp);
            }
            return new SoundMetaList()
            {
                metas = sounds,
            };
        }
    }
}
