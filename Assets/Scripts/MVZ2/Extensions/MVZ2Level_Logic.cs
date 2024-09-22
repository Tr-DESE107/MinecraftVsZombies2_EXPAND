using MVZ2.Level.Components;
using PVZEngine.Level;

namespace MVZ2.Extensions
{
    public static partial class MVZ2Level
    {
        public static ILogicComponent GetLogicComponent(this LevelEngine level)
        {
            return level.GetComponent<ILogicComponent>();
        }
        public static void BeginLevel(this LevelEngine level, string transition)
        {
            var component = level.GetLogicComponent();
            component.BeginLevel(transition);
        }
        public static void StopLevel(this LevelEngine level)
        {
            var component = level.GetLogicComponent();
            component.StopLevel();
        }
    }
}
