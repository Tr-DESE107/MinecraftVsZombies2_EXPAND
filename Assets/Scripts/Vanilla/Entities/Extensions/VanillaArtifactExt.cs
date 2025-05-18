using MVZ2Logic.Artifacts;
using PVZEngine;
using Tools;

namespace MVZ2.Vanilla.Artifacts
{
    public static class VanillaArtifactExt
    {
        public static readonly PropertyMeta RNG = new PropertyMeta("rng");
        public static RandomGenerator GetRNG(this Artifact artifact)
        {
            return artifact.GetProperty<RandomGenerator>(RNG);
        }
        public static void SetRNG(this Artifact artifact, RandomGenerator value)
        {
            artifact.SetProperty(RNG, value);
        }
    }
}
