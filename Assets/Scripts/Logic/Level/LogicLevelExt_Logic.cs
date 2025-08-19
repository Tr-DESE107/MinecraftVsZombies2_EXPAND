using System.Threading.Tasks;
using MVZ2Logic.Level.Components;
using PVZEngine.Level;

namespace MVZ2Logic.Level
{
    public static partial class LogicLevelExt
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
        public static void SaveStateData(this LevelEngine level)
        {
            var component = level.GetLogicComponent();
            component.SaveStateData();
        }
        public static Task ReloadLevel(this LevelEngine level)
        {
            var component = level.GetLogicComponent();
            return component.ReloadLevel();
        }
        public static bool IsGamePaused(this LevelEngine level)
        {
            var component = level.GetLogicComponent();
            return component.IsGamePaused();
        }
        public static bool IsGameStarted(this LevelEngine level)
        {
            var component = level.GetLogicComponent();
            return component.IsGameStarted();
        }
        public static bool IsGameOver(this LevelEngine level)
        {
            var component = level.GetLogicComponent();
            return component.IsGameOver();
        }
        public static bool IsGameRunning(this LevelEngine level)
        {
            var component = level.GetLogicComponent();
            return component.IsGameRunning();
        }
    }
}
