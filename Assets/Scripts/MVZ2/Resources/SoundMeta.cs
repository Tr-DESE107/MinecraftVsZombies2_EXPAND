using System.Xml;
using PVZEngine;
using UnityEngine;

namespace MVZ2
{
    public class SoundMeta
    {
        public string name;
        public AudioSample[] samples;
        public int priority;
        public int maxCount;

        public AudioSample GetRandomSample()
        {
            return samples[Random.Range(0, samples.Length)];
        }
        public static SoundMeta FromXmlNode(XmlNode node)
        {
            var name = node.GetAttribute("name");
            var priority = node.GetAttributeInt("priority") ?? 0;
            var maxCount = node.GetAttributeInt("maxCount") ?? 0;
            var samples = new AudioSample[node.ChildNodes.Count];
            for (int i = 0; i < samples.Length; i++)
            {
                samples[i] = AudioSample.FromXmlNode(node.ChildNodes[i]);
            }
            return new SoundMeta()
            {
                name = name,
                priority = priority,
                maxCount = maxCount,
                samples = samples
            };
        }
    }
}
