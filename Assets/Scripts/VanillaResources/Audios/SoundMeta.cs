using System.Xml;
using UnityEngine;

namespace MVZ2Logic.Audios
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
    }
}
