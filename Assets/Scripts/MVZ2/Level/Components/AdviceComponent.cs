﻿using System;
using System.Linq;
using MVZ2.Vanilla;
using MVZ2Logic.Level.Components;
using PVZEngine;
using PVZEngine.Level;

namespace MVZ2.Level.Components
{
    public class AdviceComponent : MVZ2Component, IAdviceComponent
    {
        public AdviceComponent(LevelEngine level, LevelController controller) : base(level, componentID, controller)
        {
        }

        public void ShowAdvice(string context, string textKey, int priority, int timeout, params string[] args)
        {
            ShowAdvicePlural(context, textKey, null, 0, priority, timeout, args);
        }
        public void ShowAdvicePlural(string context, string textKey, string textPlural, long pluralNum, int priority, int timeout, params string[] args)
        {
            if (AdvicePriority > priority && AdviceTimeout != 0)
                return;
            AdviceContext = context;
            AdviceKey = textKey;
            AdvicePriority = priority;
            AdviceTimeout = timeout;
            AdviceArgs = args;
            AdvicePluralKey = textPlural;
            AdvicePluralNum = pluralNum;
            var ui = Controller.GetUIPreset();
            ui.ShowAdvice(GetAdvice());

        }
        public void HideAdvice()
        {
            AdviceContext = null;
            AdviceKey = null;
            AdvicePriority = 0;
            AdviceTimeout = 0;
            AdviceArgs = Array.Empty<string>();
            AdvicePluralKey = null;
            AdvicePluralNum = 0;
            var ui = Controller.GetUIPreset();
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
                advicePluralKey = AdvicePluralKey,
                advicePluralNum = AdvicePluralNum,
                advicePriority = AdvicePriority,
                adviceTimeout = AdviceTimeout,
                adviceArgs = AdviceArgs?.ToArray(),
            };
        }
        public override void LoadSerializable(ISerializableLevelComponent seri)
        {
            if (seri is not SerializableAdviceComponent comp)
                return;
            AdviceContext = comp.adviceContext;
            AdviceKey = comp.adviceKey;
            AdvicePluralKey = comp.advicePluralKey;
            AdvicePluralNum = comp.advicePluralNum;
            AdvicePriority = comp.advicePriority;
            AdviceTimeout = comp.adviceTimeout;
            AdviceArgs = comp.adviceArgs?.ToArray();
        }
        public override void PostLevelLoad()
        {
            base.PostLevelLoad();
            if (AdviceTimeout != 0)
            {
                var ui = Controller.GetUIPreset();
                ui.ShowAdvice(GetAdvice());
            }
        }
        public string GetAdvice()
        {
            if (string.IsNullOrEmpty(AdvicePluralKey))
                return Level.Localization.GetTextParticular(AdviceKey, AdviceContext, AdviceArgs);
            return Level.Localization.GetTextPluralParticular(AdviceKey, AdvicePluralKey, AdvicePluralNum, AdviceContext, AdviceArgs);
        }
        public string AdviceContext { get; private set; }
        public string AdviceKey { get; private set; }
        public string AdvicePluralKey { get; private set; }
        public long AdvicePluralNum { get; private set; }
        public string[] AdviceArgs { get; private set; } = Array.Empty<string>();
        public int AdvicePriority { get; private set; }
        public int AdviceTimeout { get; private set; }
        public static readonly NamespaceID componentID = new NamespaceID(VanillaMod.spaceName, "advice");
    }
    [Serializable]
    public class SerializableAdviceComponent : ISerializableLevelComponent
    {
        public string adviceContext;
        public string adviceKey;
        public string advicePluralKey;
        public long advicePluralNum;
        public int advicePriority;
        public int adviceTimeout;
        public string[] adviceArgs;
    }
}
