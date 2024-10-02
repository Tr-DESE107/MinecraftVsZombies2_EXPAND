using System.Collections;
using System.Threading.Tasks;
using MVZ2.Games;
using MVZ2.Managers;
using PVZEngine;
using UnityEngine;

namespace MVZ2
{
    public static class Global
    {
        public static bool IsMobile()
        {
            return Main.IsMobile();
        }

        public static void GotoMainmenu()
        {
            Main.Scene.DisplayPage(MainScenePageType.Mainmenu);
        }

        public static void GotoMainmenuOrMap()
        {
            Main.GotoMapOrMainmenu();
        }
        public static Coroutine StartCoroutine(IEnumerator enumerator)
        {
            return Main.CoroutineManager.StartCoroutine(enumerator);
        }
        public static Coroutine StartCoroutine(Task task)
        {
            return StartCoroutine(task.ToCoroutineFunc());
        }

        public static void FadeMusic(float target, float duration)
        {
            Main.MusicManager.StartFade(target, duration);
        }
        public static void SetMusicVolume(float volume)
        {
            Main.MusicManager.SetVolume(volume);
        }

        public static IEnumerator GotoLevel()
        {
            yield return Main.LevelManager.GotoLevelSceneAsync().ToCoroutineFunc();
            Main.Scene.HidePages();
        }
        public static void InitLevel(NamespaceID areaId, NamespaceID stageId, float introDelay = 0)
        {
            Main.LevelManager.InitLevel(areaId, stageId, introDelay);
        }

        public static IEnumerator DisplayChapterTransition(NamespaceID chapterID)
        {
            return Main.Scene.DisplayChapterTransitionAsync(chapterID).ToCoroutineFunc();
        }
        public static void HideChapterTransition()
        {
            Main.Scene.HideChapterTransition();
        }
        public static void SetBlackScreen(float value)
        {
            Main.Scene.SetBlackScreen(value);
        }
        public static void FadeBlackScreen(float target, float duration)
        {
            Main.Scene.FadeBlackScreen(target, duration);
        }

        public static string BuiltinNamespace => Main.BuiltinNamespace;
        public static float LawnToTransScale => Main.LevelManager.LawnToTransScale;
        public static Game Game => Main.Game;
        private static MainManager Main => MainManager.Instance;
    }
}
