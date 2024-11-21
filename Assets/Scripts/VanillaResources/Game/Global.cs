using System.Collections;
using System.Threading.Tasks;
using MVZ2.Games;
using MVZ2.Logic.Scenes;
using PVZEngine;
using UnityEngine;

namespace MVZ2
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
        public static Coroutine StartCoroutine(Task task)
        {
            return StartCoroutine(task.ToCoroutineFunc());
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
            Main.GotoMapOrMainmenu();
        }
        public static IEnumerator GotoLevel()
        {
            yield return Level.GotoLevelSceneAsync().ToCoroutineFunc();
            Scene.HidePages();
        }
        public static void GotoMainmenu()
        {
            Scene.DisplayPage(MainScenePageType.Mainmenu);
        }
        public static IEnumerator DisplayChapterTransition(NamespaceID chapterID)
        {
            return Scene.DisplayChapterTransitionAsync(chapterID).ToCoroutineFunc();
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

        private static IMainManager Main { get; set; }
        public static string BuiltinNamespace => Game.DefaultNamespace;
        public static Game Game => Main.Game;
        private static ISceneController Scene => Main.Scene;
        private static IMusicManager Music => Main.Music;
        private static ILevelManager Level => Main.Level;
    }
    public interface IMainManager
    {
        bool IsMobile();
        Coroutine StartCoroutine(IEnumerator enumerator);
        void GotoMapOrMainmenu();
        Game Game { get; }
        ISceneController Scene { get; }
        IMusicManager Music { get; }
        ILevelManager Level { get; }
    }
    public interface ISceneController
    {
        void DisplayPage(MainScenePageType type);
        void HidePages();
        void FadeBlackScreen(float target, float duration);
        void SetBlackScreen(float value);
        void HideChapterTransition();
        Task DisplayChapterTransitionAsync(NamespaceID chapterID);
    }
    public interface IMusicManager
    {
        void StartFade(float target, float duration);
        void SetVolume(float volume);
    }
    public interface ILevelManager
    {
        void InitLevel(NamespaceID areaId, NamespaceID stageId, float introDelay = 0);
        Task GotoLevelSceneAsync();
    }
}
