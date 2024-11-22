using MVZ2Logic.Level.Components;
using PVZEngine.Level;

namespace MVZ2Logic.Level
{
    public static partial class MVZ2Level
    {
        public static ILogicComponent GetLogicComponent(this LevelEngine level)
        {
            return level.GetComponent<ILogicComponent>();
        }
        public static void BeginLevel(this LevelEngine level)
        {
            var component = level.GetLogicComponent();
            component.BeginLevel();
        }
        public static void StopLevel(this LevelEngine level)
        {
            var component = level.GetLogicComponent();
            component.StopLevel();
        }
    }
}
