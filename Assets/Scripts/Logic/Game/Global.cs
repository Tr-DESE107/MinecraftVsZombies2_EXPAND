using System;
using System.Collections;
using MVZ2Logic.Games;
using MVZ2Logic.Scenes;
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
        }
        public static bool IsMobile()
        {
            return Main.IsMobile();
        }

        public static Coroutine StartCoroutine(IEnumerator enumerator)
        {
            return Main.StartCoroutine(enumerator);
        }
        public static void FadeMusic(float target, float duration)
        {
            Music.StartFade(target, duration);
        }
        public static void SetMusicVolume(float volume)
        {
            Music.SetVolume(volume);
        }
        public static void StopMusic()
        {
            Music.Stop();
        }
        public static void InitLevel(NamespaceID areaId, NamespaceID stageId, float introDelay = 0)
        {
            Level.InitLevel(areaId, stageId, introDelay);
        }
        public static void ShowDialog(string title, string desc, string[] options, Action<int> onSelect = null)
        {
            Scene.ShowDialog(title, desc, options, onSelect);
        }

        public static void GotoMainmenuOrMap()
        {
            Scene.GotoMapOrMainmenu();
        }
        public static IEnumerator GotoLevel()
        {
            yield return Level.GotoLevelSceneCoroutine();
            Scene.HidePages();
        }
        public static void GotoMainmenu()
        {
            Scene.DisplayPage(MainScenePageType.Mainmenu);
        }
        public static void GotoMap(NamespaceID mapID)
        {
            Scene.DisplayMap(mapID);
        }
        public static Coroutine DisplayChapterTransition(NamespaceID chapterID, bool end = false)
        {
            return Scene.DisplayChapterTransitionCoroutine(chapterID, end);
        }
        public static void HideChapterTransition()
        {
            Scene.HideChapterTransition();
        }
        public static void SetScreenCoverColor(Color value)
        {
            Scene.SetScreenCoverColor(value);
        }
        public static void FadeScreenCoverColor(Color target, float duration)
        {
            Scene.FadeScreenCoverColor(target, duration);
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
        public static string BuiltinNamespace => Game.DefaultNamespace;
        public static IGame Game => Main.Game;
        private static ISceneController Scene => Main.Scene;
        private static IMusicManager Music => Main.Music;
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
    }
    public interface IMainManager
    {
        bool IsMobile();
        Coroutine StartCoroutine(IEnumerator enumerator);
        IGame Game { get; }
        ISceneController Scene { get; }
        IMusicManager Music { get; }
        IDebugManager Debugs { get; }
    }
    public interface IGlobalModels
    {
        bool ModelExists(NamespaceID id);
    }
    public interface ISceneController
    {
        void GotoMapOrMainmenu();
        void DisplayPage(MainScenePageType type);
        void DisplayMap(NamespaceID mapID);
        void HidePages();
        void FadeScreenCoverColor(Color target, float duration);
        void SetScreenCoverColor(Color value);
        void HideChapterTransition();
        void ShowDialog(string title, string desc, string[] options, Action<int> onSelect = null);
        void Print(string text);
        Coroutine DisplayChapterTransitionCoroutine(NamespaceID chapterID, bool end);
    }
    public interface IMusicManager
    {
        void StartFade(float target, float duration);
        void SetVolume(float volume);
        void Stop();
    }
    }
    public interface IDebugManager
    {
        string[] GetCommandHistory();
        void ExecuteCommand(string command);
        void ClearConsole();
}
