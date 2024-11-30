using System.Xml;
using MVZ2.IO;
using UnityEngine;

namespace MVZ2.Metas
{
    public class SoundMeta
    {
        public string name;
        public AudioSample[] samples;
        public int priority;
        public int maxCount;

        public static SoundMeta FromXmlNode(XmlNode node, string defaultNsp)
        {
            var name = node.GetAttribute("name");
            var priority = node.GetAttributeInt("priority") ?? 128;
            var maxCount = node.GetAttributeInt("maxCount") ?? 2;
            var samples = new AudioSample[node.ChildNodes.Count];
            for (int i = 0; i < samples.Length; i++)
            {
                samples[i] = AudioSample.FromXmlNode(node.ChildNodes[i], defaultNsp);
            }
            return new SoundMeta()
            {
                name = name,
                priority = priority,
                maxCount = maxCount,
                samples = samples
            };
        }
        public AudioSample GetRandomSample()
        {
            return samples[Random.Range(0, samples.Length)];
        }
    }
}
