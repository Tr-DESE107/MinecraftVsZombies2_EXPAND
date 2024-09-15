using PVZEngine.Level;

namespace MVZ2
{
    public static partial class MVZ2Level
    {
        public static LogicComponent GetLogicComponent(this LevelEngine level)
        {
            return level.GetComponent<LogicComponent>();
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
