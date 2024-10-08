using PVZEngine;
using PVZEngine.Callbacks;
using PVZEngine.Level;

namespace MVZ2.GameContent
{
    public static class BuiltinCallbacks
    {
        public readonly static CallbackActionList<LevelEngine> PostHugeWaveApproach = new();
        public readonly static CallbackActionList<LevelEngine> PostFinalWave = new();
        public readonly static CallbackActionList<ITalkSystem, string, string[]> TalkAction = new();
        public readonly static NamespaceID POST_ADD_LEVEL_COMPONENTS = Get("post_add_level_components");
        public static NamespaceID Get(string path)
        {
            return new NamespaceID(Builtin.spaceName, path);
        }
    }
}
