using MVZ2Logic.Talk;
using PVZEngine;
using PVZEngine.Callbacks;
using PVZEngine.Level;
using PVZEngine.Triggers;

namespace MVZ2.Vanilla.Callbacks
{
    public static class VanillaCallbacks
    {
        public delegate void PostHugeWaveApproach(LevelEngine level);
        public delegate void PostFinalWave(LevelEngine level);
        public delegate void TalkAction(ITalkSystem system, string action, string[] parameters);
        public readonly static CallbackReference<PostHugeWaveApproach> POST_HUGE_WAVE_APPROACH = new();
        public readonly static CallbackReference<PostFinalWave> POST_FINAL_WAVE = new();
        public readonly static CallbackReference<TalkAction> TALK_ACTION = new();
        public readonly static NamespaceID POST_ADD_LEVEL_COMPONENTS = Get("post_add_level_components");
        public static NamespaceID Get(string path)
        {
            return new NamespaceID(VanillaMod.spaceName, path);
        }
    }
}
