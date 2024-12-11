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
        public static bool CanStartTalk(this LevelEngine level, NamespaceID groupId, int section)
        {
            var component = level.GetTalkComponent();
            return component.CanStartTalk(groupId, section);
        }
        public static void StartTalk(this LevelEngine level, NamespaceID groupId, int section, float delay = 0, Action onEnd = null)
        {
            var component = level.GetTalkComponent();
            component.StartTalk(groupId, section, delay, onEnd);
        }
        public static void CanSkipTalk(this LevelEngine level, NamespaceID groupId, int section)
        {
            var component = level.GetTalkComponent();
            component.CanStartTalk(groupId, section);
        }
        public static void SkipTalk(this LevelEngine level, NamespaceID groupId, int section, Action onSkipped = null)
        {
            var component = level.GetTalkComponent();
            component.SkipTalk(groupId, section, onSkipped);
        }
        public static void SimpleStartTalk(this LevelEngine level, NamespaceID groupId, int section, float delay = 0, Action onSkipped = null, Action onStarted = null, Action onEnd = null)
        {
            var component = level.GetTalkComponent();
            component.SimpleStartTalk(groupId, section, delay, onSkipped, onStarted, onEnd);
        }
    }
}
