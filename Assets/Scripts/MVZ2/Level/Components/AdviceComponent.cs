using System;
using log4net.Core;
using MVZ2.GameContent;
using MVZ2.Level.Components;
using PVZEngine;
using PVZEngine.Level;

namespace MVZ2.Level
{
    public class AdviceComponent : MVZ2Component, IAdviceComponent
    {
        public AdviceComponent(LevelEngine level, LevelController controller) : base(level, componentID, controller)
        {
        }

        public void ShowAdvice(string context, string textKey, int priority, int timeout)
        {
            if (AdvicePriority >= priority && AdviceTimeout == 0)
                return;
            AdviceContext = context;
            AdviceKey = textKey;
            AdvicePriority = priority;
            AdviceTimeout = timeout;
            var ui = Controller.GetLevelUI();
            ui.ShowAdvice(GetAdvice());

        }
        public void HideAdvice()
        {
            AdviceContext = null;
            AdviceKey = null;
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
                adviceContext = AdviceContext,
                adviceKey = AdviceKey,
                advicePriority = AdvicePriority,
                adviceTimeout = AdviceTimeout
            };
        }
        public override void LoadSerializable(ISerializableLevelComponent seri)
        {
            if (seri is not SerializableAdviceComponent comp)
                return;
            AdviceContext = comp.adviceContext;
            AdviceKey = comp.adviceKey;
            AdvicePriority = comp.advicePriority;
            AdviceTimeout = comp.adviceTimeout;
        }
        public override void PostLevelLoad()
        {
            base.PostLevelLoad();
            if (AdviceTimeout != 0)
            {
                var ui = Controller.GetLevelUI();
                ui.ShowAdvice(GetAdvice());
            }
        }
        public string GetAdvice()
        {
            return Level.Translator.GetTextParticular(AdviceKey, AdviceContext);
        }
        public string AdviceContext { get; private set; }
        public string AdviceKey { get; private set; }
        public int AdvicePriority { get; private set; }
        public int AdviceTimeout { get; private set; }
        public static readonly NamespaceID componentID = new NamespaceID(Builtin.spaceName, "advice");
    }
    [Serializable]
    public class SerializableAdviceComponent : ISerializableLevelComponent
    {
        public string adviceContext;
        public string adviceKey;
        public int advicePriority;
        public int adviceTimeout;
    }
}
