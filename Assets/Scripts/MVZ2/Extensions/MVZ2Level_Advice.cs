using System;
using MVZ2.Level;
using PVZEngine.Level;

namespace MVZ2
{
    public static partial class MVZ2Level
    {
        public static AdviceComponent GetAdviceComponent(this LevelEngine level)
        {
            return level.GetComponent<AdviceComponent>();
        }
        public static void ShowAdvice(this LevelEngine level, string text, int priority, int timeout)
        {
            var component = level.GetAdviceComponent();
            component.ShowAdvice(text, priority, timeout);
        }
        public static void HideAdvice(this LevelEngine level)
        {
            var component = level.GetAdviceComponent();
            component.HideAdvice();
        }
    }
}
