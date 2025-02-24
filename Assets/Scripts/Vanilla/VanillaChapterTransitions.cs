using System.Collections;
using MVZ2Logic;
using PVZEngine;
using UnityEngine;

namespace MVZ2.Vanilla
{
    public static class VanillaChapterTransitions
    {
        public readonly static NamespaceID halloween = Get("halloween");
        public readonly static NamespaceID dream = Get("dream");
        public readonly static NamespaceID castle = Get("castle");
        private static NamespaceID Get(string name)
        {
            return new NamespaceID(VanillaMod.spaceName, name);
        }
        public static IEnumerator TransitionToLevel(NamespaceID transition, NamespaceID areaID, NamespaceID stageID)
        {
            Global.FadeMusic(0, 2);
            Global.SetBlackScreen(0);
            Global.FadeBlackScreen(1, 1);

            yield return new WaitForSeconds(2);

            Global.StopMusic();
            Global.SetBlackScreen(0);
            yield return Global.DisplayChapterTransition(transition);

            Global.SetBlackScreen(1);
            yield return Global.GotoLevel();

            yield return new WaitForSeconds(2);

            Global.FadeBlackScreen(0, 1);
            Global.SetMusicVolume(1);
            Global.InitLevel(areaID, stageID, 1);
            Global.HideChapterTransition();
        }
        public static IEnumerator TransitionEndToMap(NamespaceID transition, NamespaceID mapID)
        {
            Global.FadeBlackScreen(1, 0.5f);
            yield return new WaitForSeconds(2);

            Global.SetBlackScreen(0);
            yield return Global.DisplayChapterTransition(transition, true);

            Global.FadeMusic(0, 1);
            yield return new WaitForSeconds(2);
            Global.SetMusicVolume(1);
            Global.SetBlackScreen(1);
            Global.FadeBlackScreen(0, 1);
            Global.HideChapterTransition();
            Global.GotoMap(mapID);
        }
    }
}
