using System;
using MVZ2.GameContent;
using PVZEngine;
using PVZEngine.Definitions;
using PVZEngine.Level;

namespace MVZ2
{
    public static partial class MVZ2Level
    {
        public static TalkComponent GetTalkComponent(this LevelEngine level)
        {
            return level.GetComponent<TalkComponent>();
        }
        public static void StartTalk(this LevelEngine level, NamespaceID groupId, int section, float delay = 0)
        {
            var component = level.GetTalkComponent();
            component.StartTalk(groupId, section, delay);
        }
    }
}
