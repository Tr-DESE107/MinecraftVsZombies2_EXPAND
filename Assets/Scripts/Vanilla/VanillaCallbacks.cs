using PVZEngine.Callbacks;
using PVZEngine.Level;

namespace MVZ2.Vanilla
{
    public static class VanillaCallbacks
    {
        public readonly static CallbackActionList<Entity> PostContraptionEvoked = new();
        public readonly static CallbackActionList<LevelEngine> PostHugeWaveApproach = new();
        public readonly static CallbackActionList<LevelEngine> PostFinalWave = new();
        public readonly static CallbackActionList<ITalkSystem, string, string[]> TalkAction = new();
    }
}
