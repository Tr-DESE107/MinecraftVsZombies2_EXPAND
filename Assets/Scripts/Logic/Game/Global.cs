using MVZ2Logic.Games;

namespace MVZ2Logic
{
    public static class Global
    {
        public static void Init(GlobalParams param)
        {
            Models = param.models;
            Almanac = param.almanac;
            Saves = param.saveData;
            Options = param.options;
            Input = param.input;
            Level = param.level;
            Music = param.music;
            GUI = param.gui;
            Scene = param.scene;
            Game = param.game;
            Localization = param.localization;
        }
        public static void Print(string text)
        {
            Scene.Print(text);
        }


        #region 调试
        public static string[] GetCommandHistory()
        {
            return Debugs.GetCommandHistory();
        }
        public static void ExecuteCommand(string command)
        {
            Debugs.ExecuteCommand(command);
        }
        public static void ClearConsole()
        {
            Debugs.ClearConsole();
        }
        #endregion
        public static IGlobalModels Models { get; private set; }
        public static IGlobalAlmanac Almanac { get; private set; }
        public static IGlobalSaveData Saves { get; private set; }
        public static IGlobalOptions Options { get; private set; }
        public static IGlobalInput Input { get; private set; }
        public static IGlobalLevel Level { get; private set; }
        public static IGlobalMusic Music { get; private set; }
        public static IGlobalGUI GUI { get; private set; }
        public static IGlobalScene Scene { get; private set; }
        public static IGlobalGame Game { get; private set; }
        public static IGlobalLocalization Localization { get; private set; }
        public static string BuiltinNamespace => Game.DefaultNamespace;
    }
    public struct GlobalParams
    {
        public IGlobalGame game;
        public IGlobalModels models;
        public IGlobalAlmanac almanac;
        public IGlobalSaveData saveData;
        public IGlobalOptions options;
        public IGlobalInput input;
        public IGlobalLevel level;
        public IGlobalMusic music;
        public IGlobalGUI gui;
        public IGlobalScene scene;
        public IGlobalLocalization localization;
    }
        IDebugManager Debugs { get; }
        void Print(string text);
    }
    public interface IDebugManager
    {
        string[] GetCommandHistory();
        void ExecuteCommand(string command);
        void ClearConsole();
}
