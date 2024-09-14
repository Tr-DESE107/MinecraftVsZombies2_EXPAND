using PVZEngine.Base;
using PVZEngine.Callbacks;
using PVZEngine.LevelManagement;

namespace MVZ2.Vanilla
{
    public static class VanillaCallbacks
    {
        public readonly static CallbackActionList<Entity> PostContraptionEvoked = new();
        public readonly static CallbackActionList<Level> PostHugeWaveApproach = new();
        public readonly static CallbackActionList<Level> PostFinalWave = new();
        public readonly static CallbackActionList<ITalkSystem, string, string[]> TalkAction = new();
    }
}
