﻿using MVZ2Logic.Level.Components;
using PVZEngine.Level;

namespace MVZ2Logic.Level
{
    public static partial class LogicLevelExt
    {
        public static IAdviceComponent GetAdviceComponent(this LevelEngine level)
        {
            return level.GetComponent<IAdviceComponent>();
        }
        public static void ShowAdvice(this LevelEngine level, string context, string textKey, int priority, int timeout, params string[] args)
        {
            var component = level.GetAdviceComponent();
            component.ShowAdvice(context, textKey, priority, timeout, args);
        }
        public static void ShowAdvicePlural(this LevelEngine level, string context, string textKey, string textPlural, long n, int priority, int timeout, params string[] args)
        {
            var component = level.GetAdviceComponent();
            component.ShowAdvicePlural(context, textKey, textPlural, n, priority, timeout, args);
        }
        public static void ShowAdvicePlural(this LevelEngine level, string context, string textKey, long n, int priority, int timeout, params string[] args)
        {
            level.ShowAdvicePlural(context, textKey, textKey, n, priority, timeout, args);
        }
        public static void HideAdvice(this LevelEngine level)
        {
            var component = level.GetAdviceComponent();
            component.HideAdvice();
        }
    }
}
