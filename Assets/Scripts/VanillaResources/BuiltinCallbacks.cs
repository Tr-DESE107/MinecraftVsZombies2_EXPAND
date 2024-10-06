using PVZEngine.Callbacks;
using PVZEngine.Level;

namespace MVZ2.GameContent
{
    public static class BuiltinCallbacks
    {
        public readonly static CallbackActionList<LevelEngine> PostHugeWaveApproach = new();
        public readonly static CallbackActionList<LevelEngine> PostFinalWave = new();
        public readonly static CallbackActionList<ITalkSystem, string, string[]> TalkAction = new();
    }
}
