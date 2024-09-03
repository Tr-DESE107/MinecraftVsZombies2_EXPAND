using PVZEngine.Callbacks;
using PVZEngine.LevelManaging;

namespace MVZ2.Vanilla
{
    public static class VanillaCallbacks
    {
        public readonly static CallbackActionList<Entity> PostContraptionEvoked = new();
        public readonly static CallbackActionList<Level> PostHugeWaveApproach = new();
        public readonly static CallbackActionList<Level> PostFinalWave = new();
    }
}
