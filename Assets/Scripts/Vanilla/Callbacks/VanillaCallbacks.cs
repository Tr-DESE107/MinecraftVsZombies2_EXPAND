using MVZ2Logic;
using MVZ2Logic.Talk;
using PVZEngine.Callbacks;
using UnityEngine;

namespace MVZ2.Vanilla.Callbacks
{
    public static class VanillaCallbacks
    {
        public struct TalkActionParams
        {
            public ITalkSystem system;
            public string action;
            public string[] parameters;

            public TalkActionParams(ITalkSystem system, string action, string[] parameters)
            {
                this.system = system;
                this.action = action;
                this.parameters = parameters;
            }
        }
        public readonly static CallbackType<TalkActionParams> TALK_ACTION = new();

        public struct PostPointerActionParams
        {
            public int type;
            public int index;
            public Vector2 screenPos;
            public PointerPhase phase;

            public PostPointerActionParams(int type, int index, Vector2 screenPos, PointerPhase phase)
            {
                this.type = type;
                this.index = index;
                this.screenPos = screenPos;
                this.phase = phase;
            }
        }
        public readonly static CallbackType<PostPointerActionParams> POST_POINTER_ACTION = new();
    }
}
