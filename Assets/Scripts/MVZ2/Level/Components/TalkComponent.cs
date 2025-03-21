using System;
using MVZ2.Vanilla;
using MVZ2Logic.Level.Components;
using PVZEngine;
using PVZEngine.Level;

namespace MVZ2.Level.Components
{
    public partial class TalkComponent : MVZ2Component, ITalkComponent
    {
        public TalkComponent(LevelEngine level, LevelController controller) : base(level, componentID, controller)
        {
        }
        public bool CanStartTalk(NamespaceID id, int section)
        {
            return Controller.CanStartTalk(id, section);
        }
        public void StartTalk(NamespaceID id, int section, float delay = 1, Action onEnd = null)
        {
            Controller.StartTalk(id, section, delay, onEnd);
        }
        public bool WillSkipTalk(NamespaceID id, int section)
        {
            return Controller.WillSkipTalk(id, section);
        }
        public void SkipTalk(NamespaceID id, int section, Action onSkipped = null)
        {
            Controller.SkipTalk(id, section, onSkipped);
        }
        public static readonly NamespaceID componentID = new NamespaceID(VanillaMod.spaceName, "talk");
    }
}