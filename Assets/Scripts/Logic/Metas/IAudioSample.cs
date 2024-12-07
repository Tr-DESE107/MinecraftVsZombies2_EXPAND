using PVZEngine;

namespace MVZ2Logic.Audios
{
    public interface IAudioSample
    {
        public NamespaceID Path { get; }
        public float Weight { get; }
    }
}
