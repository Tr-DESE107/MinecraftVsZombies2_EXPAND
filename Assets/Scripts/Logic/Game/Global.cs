using System.Collections;
using MVZ2Logic.Games;
using PVZEngine;
using UnityEngine;

namespace MVZ2Logic
{
    public static class Global
    {
        public static void Init(GlobalParams param)
        {
            Main = param.main;
            Models = param.models;
            Almanac = param.almanac;
            Saves = param.saveData;
            Options = param.options;
            Input = param.input;
            Level = param.level;
            Music = param.music;
            GUI = param.gui;
            Scene = param.scene;
        }
        public static bool IsMobile()
        {
            return Main.IsMobile();
        }

        public static Coroutine StartCoroutine(IEnumerator enumerator)
        {
            return Main.StartCoroutine(enumerator);
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
        private static IMainManager Main { get; set; }
        public static IGlobalModels Models { get; private set; }
        public static IGlobalAlmanac Almanac { get; private set; }
        public static IGlobalSaveData Saves { get; private set; }
        public static IGlobalOptions Options { get; private set; }
        public static IGlobalInput Input { get; private set; }
        public static IGlobalLevel Level { get; private set; }
        public static IGlobalMusic Music { get; private set; }
        public static IGlobalGUI GUI { get; private set; }
        public static IGlobalScene Scene { get; private set; }
        public static string BuiltinNamespace => Game.DefaultNamespace;
        public static IGame Game => Main.Game;
    }
    public struct GlobalParams
    {
        public IMainManager main;
        public IGlobalModels models;
        public IGlobalAlmanac almanac;
        public IGlobalSaveData saveData;
        public IGlobalOptions options;
        public IGlobalInput input;
        public IGlobalLevel level;
        public IGlobalMusic music;
        public IGlobalGUI gui;
        public IGlobalScene scene;
    }
    public interface IMainManager
    {
        bool IsMobile();
        Coroutine StartCoroutine(IEnumerator enumerator);
        IGame Game { get; }
        IDebugManager Debugs { get; }
    }
    public interface IGlobalModels
    {
        bool ModelExists(NamespaceID id);
    }
        void Print(string text);
    }
    public interface IDebugManager
    {
        string[] GetCommandHistory();
        void ExecuteCommand(string command);
        void ClearConsole();
}
