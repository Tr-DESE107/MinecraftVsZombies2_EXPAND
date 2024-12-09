using System;
using MVZ2Logic.Level.Components;
using PVZEngine;
using PVZEngine.Level;

namespace MVZ2Logic.Level
{
    public static partial class LogicLevelExt
    {
        public static ITalkComponent GetTalkComponent(this LevelEngine level)
        {
            return level.GetComponent<ITalkComponent>();
        }
        public static void StartTalk(this LevelEngine level, NamespaceID groupId, int section, float delay = 0)
        {
            var component = level.GetTalkComponent();
            component.StartTalk(groupId, section, delay);
        }
        public static void TryStartTalk(this LevelEngine level, NamespaceID groupId, int section, float delay = 0, Action<bool> onFinished = null)
        {
            var component = level.GetTalkComponent();
            component.TryStartTalk(groupId, section, delay, onFinished);
        }
    }
}
