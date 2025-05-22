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
        public static void Init(IMainManager main)
        {
            Main = main;
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
        public static int GetTouchCount()
        {
            return Input.touchCount;
        }
        public static Vector2 GetTouchDelta(int index)
        {
            return Input.GetTouch(index).deltaPosition;
        }
        public static Vector2 GetTouchPosition(int index)
        {
            return Input.GetTouch(index).position;
        }
        public static bool IsMousePresent()
        {
            return Input.mousePresent;
        }
        public static Vector2 GetMousePosition()
        {
            return Input.mousePosition;
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
        public static void SetBlackScreen(float value)
        {
            Scene.SetBlackScreen(value);
        }
        public static void FadeBlackScreen(float target, float duration)
        {
            Scene.FadeBlackScreen(target, duration);
        }

        #region 选项
        public static bool HasBloodAndGore()
        {
            return Options.HasBloodAndGore();
        }
        #endregion

        #region 统计
        public static long GetSaveStat(NamespaceID category, NamespaceID entry)
        {
            return Saves.GetSaveStat(category, entry);
        }
        public static void SetSaveStat(NamespaceID category, NamespaceID entry, long value)
        {
            Saves.SetSaveStat(category, entry, value);
        }
        public static void AddSaveStat(NamespaceID category, NamespaceID entry, long value)
        {
            SetSaveStat(category, entry, GetSaveStat(category, entry) + value);
        }
        #endregion
        private static IMainManager Main { get; set; }
        public static string BuiltinNamespace => Game.DefaultNamespace;
        public static IGame Game => Main.Game;
        private static ISceneController Scene => Main.Scene;
        private static IMusicManager Music => Main.Music;
        private static ILevelManager Level => Main.Level;
        private static IOptionsManager Options => Main.Options;
        private static IGlobalSave Saves => Main.Saves;
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
        IGlobalSave Saves { get; }
    }
    public interface ISceneController
    {
        void GotoMapOrMainmenu();
        void DisplayPage(MainScenePageType type);
        void DisplayMap(NamespaceID mapID);
        void HidePages();
        void FadeBlackScreen(float target, float duration);
        void SetBlackScreen(float value);
        void HideChapterTransition();
        void ShowDialog(string title, string desc, string[] options, Action<int> onSelect = null);
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
    }
    public interface IGlobalSave
    {
        long GetSaveStat(NamespaceID category, NamespaceID entry);
        void SetSaveStat(NamespaceID category, NamespaceID entry, long value);
    }
}
