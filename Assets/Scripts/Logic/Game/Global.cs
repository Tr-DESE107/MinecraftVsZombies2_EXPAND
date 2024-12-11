using System.Collections;
using System.Threading.Tasks;
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
        public static void InitLevel(NamespaceID areaId, NamespaceID stageId, float introDelay = 0)
        {
            Level.InitLevel(areaId, stageId, introDelay);
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
        public static Coroutine DisplayChapterTransition(NamespaceID chapterID)
        {
            return Scene.DisplayChapterTransitionCoroutine(chapterID);
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
        public static void AddSaveStat(NamespaceID category, NamespaceID entry, long value)
        {
            Saves.AddSaveStat(category, entry, value);
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
        void HidePages();
        void FadeBlackScreen(float target, float duration);
        void SetBlackScreen(float value);
        void HideChapterTransition();
        Coroutine DisplayChapterTransitionCoroutine(NamespaceID chapterID);
    }
    public interface IMusicManager
    {
        void StartFade(float target, float duration);
        void SetVolume(float volume);
    }
    public interface ILevelManager
    {
        void InitLevel(NamespaceID areaId, NamespaceID stageId, float introDelay = 0);
        Coroutine GotoLevelSceneCoroutine();
    }
    public interface IOptionsManager
    {
        bool HasBloodAndGore();
    }
    public interface IGlobalSave
    {
        long GetSaveStat(NamespaceID category, NamespaceID entry);
        void AddSaveStat(NamespaceID category, NamespaceID entry, long value);
    }
}
