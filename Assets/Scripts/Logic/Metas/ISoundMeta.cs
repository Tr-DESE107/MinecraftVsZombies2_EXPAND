using UnityEngine;

namespace MVZ2Logic.Audios
{
    public interface ISoundMeta
    {
        public string Name { get; }
        public IAudioSample[] Samples { get; }
        public int Priority { get; }
        public int MaxCount { get; }

        public IAudioSample GetRandomSample()
        {
            return Samples[Random.Range(0, Samples.Length)];
        }
    }
}
