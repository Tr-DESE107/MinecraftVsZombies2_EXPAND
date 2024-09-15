using System;
using MVZ2.GameContent;
using PVZEngine;
using PVZEngine.Level;

namespace MVZ2.Level
{
    public class AdviceComponent : MVZ2Component
    {
        public AdviceComponent(LevelEngine level, LevelController controller) : base(level, componentID, controller)
        {
        }

        public void ShowAdvice(string text, int priority, int timeout)
        {
            if (AdvicePriority >= priority && AdviceTimeout == 0)
                return;
            AdvicePriority = priority;
            AdviceTimeout = timeout;
            var ui = Controller.GetLevelUI();
            ui.ShowAdvice(text);

        }
        public void HideAdvice()
        {
            AdvicePriority = 0;
            AdviceTimeout = 0;
            var ui = Controller.GetLevelUI();
            ui.HideAdvice();
        }
        public override void Update()
        {
            if (AdviceTimeout <= 0)
                return;
            AdviceTimeout--;
            if (AdviceTimeout > 0)
                return;
            HideAdvice();
        }
        public override ISerializableLevelComponent ToSerializable()
        {
            return new SerializableAdviceComponent()
            {
                advicePriority = AdvicePriority,
                adviceTimeout = AdviceTimeout
            };
        }
        public override void LoadSerializable(ISerializableLevelComponent seri)
        {
            if (seri is not SerializableAdviceComponent comp)
                return;
            AdvicePriority = comp.advicePriority;
            AdviceTimeout = comp.adviceTimeout;
        }
        public int AdvicePriority { get; private set; }
        public int AdviceTimeout { get; private set; }
        public static readonly NamespaceID componentID = new NamespaceID(Builtin.spaceName, "advice");
    }
    public class SerializableAdviceComponent : ISerializableLevelComponent
    {
        public int advicePriority;
        public int adviceTimeout;
    }
}
