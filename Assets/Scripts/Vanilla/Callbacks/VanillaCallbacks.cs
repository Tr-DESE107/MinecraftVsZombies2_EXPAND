using MVZ2Logic.Talk;
using PVZEngine;
using PVZEngine.Callbacks;
using PVZEngine.Level;

namespace MVZ2.Vanilla.Callbacks
{
    public static class VanillaCallbacks
    {
        public readonly static CallbackActionList<LevelEngine> PostHugeWaveApproach = new();
        public readonly static CallbackActionList<LevelEngine> PostFinalWave = new();
        public readonly static CallbackActionList<ITalkSystem, string, string[]> TalkAction = new();
        public readonly static NamespaceID POST_ADD_LEVEL_COMPONENTS = Get("post_add_level_components");
        public static NamespaceID Get(string path)
        {
            return new NamespaceID(VanillaMod.spaceName, path);
        }
    }
}
