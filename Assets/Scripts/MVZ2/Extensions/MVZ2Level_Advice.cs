using System;
using MVZ2.Level;
using MVZ2.Level.Components;
using PVZEngine.Level;

namespace MVZ2
{
    public static partial class MVZ2Level
    {
        public static IAdviceComponent GetAdviceComponent(this LevelEngine level)
        {
            return level.GetComponent<IAdviceComponent>();
        }
        public static void ShowAdvice(this LevelEngine level, string context, string textKey, int priority, int timeout)
        {
            var component = level.GetAdviceComponent();
            component.ShowAdvice(context, textKey, priority, timeout);
        }
        public static void HideAdvice(this LevelEngine level)
        {
            var component = level.GetAdviceComponent();
            component.HideAdvice();
        }
    }
}
