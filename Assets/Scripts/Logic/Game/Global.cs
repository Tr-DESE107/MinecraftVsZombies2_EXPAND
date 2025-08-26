using System;
using System.Collections;
using MVZ2.Logic.Level;
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
        }
        public static bool IsMobile()
        {
            return Main.IsMobile();
        }
        public static Vector2 GetPointerScreenPosition()
        {
            if (Input.touchCount > 0)
            {
                return Input.GetTouch(0).position;
            }
            return Input.mousePosition;
        }
        public static bool IsPointerDown(int type, int button)
        {
            return Main.Input.IsPointerDown(type, button);
        }
        public static bool IsPointerHolding(int type, int button)
        {
            return Main.Input.IsPointerHolding(type, button);
        }
        public static bool IsPointerUp(int type, int button)
        {
            return Main.Input.IsPointerUp(type, button);
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

        #region 选项
        public static bool HasBloodAndGore()
        {
            return Options.HasBloodAndGore();
        }
        public static bool IsTriggerSwapped()
        {
            return Options.IsTriggerSwapped();
        }
        #endregion


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
        public static string BuiltinNamespace => Game.DefaultNamespace;
        public static IGame Game => Main.Game;
        private static ISceneController Scene => Main.Scene;
        private static IMusicManager Music => Main.Music;
        private static ILevelManager Level => Main.Level;
        private static IOptionsManager Options => Main.Options;
    }
    public struct GlobalParams
    {
        public IMainManager main;
        public IGlobalModels models;
        public IGlobalAlmanac almanac;
        public IGlobalSaveData saveData;
    }
    public interface IMainManager
    {
        bool IsMobile();
        Coroutine StartCoroutine(IEnumerator enumerator);
        IGame Game { get; }
        ISceneController Scene { get; }
        IMusicManager Music { get; }
        ILevelManager Level { get; }
        IOptionsManager Options { get; }
        IInputManager Input { get; }
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
    public interface ILevelManager
    {
        void InitLevel(NamespaceID areaId, NamespaceID stageId, float introDelay = 0, LevelExitTarget exitTarget = LevelExitTarget.MapOrMainmenu);
        Coroutine GotoLevelSceneCoroutine();
    }
    public interface IOptionsManager
    {
        bool HasBloodAndGore();
        bool IsTriggerSwapped();
    }
    public interface IInputManager
    {
        bool IsPointerDown(int type, int button);
        bool IsPointerHolding(int type, int button);
        bool IsPointerUp(int type, int button);
    }
    public interface IDebugManager
    {
        string[] GetCommandHistory();
        void ExecuteCommand(string command);
        void ClearConsole();
    }
}
