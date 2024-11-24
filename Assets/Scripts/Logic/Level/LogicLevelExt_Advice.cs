using MVZ2Logic.Level.Components;
using PVZEngine.Level;

namespace MVZ2Logic.Level
{
    public static partial class LogicLevelExt
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
