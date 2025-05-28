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
        public readonly static NamespaceID mausoleum = Get("mausoleum");
        private static NamespaceID Get(string name)
        {
            return new NamespaceID(VanillaMod.spaceName, name);
        }
        public static IEnumerator TransitionToLevel(NamespaceID transition, NamespaceID areaID, NamespaceID stageID)
        {
            Global.FadeMusic(0, 2);
            Global.SetScreenCoverColor(new Color(0, 0, 0, 0));
            Global.FadeScreenCoverColor(new Color(0, 0, 0, 1), 1);

            yield return new WaitForSeconds(2);

            yield return TransitionTalkToLevel(transition, areaID, stageID);
        }
        public static IEnumerator TransitionTalkToLevel(NamespaceID transition, NamespaceID areaID, NamespaceID stageID)
        {
            Global.StopMusic();
            Global.SetScreenCoverColor(new Color(0, 0, 0, 1));
            Global.FadeScreenCoverColor(new Color(0, 0, 0, 0), 0.5f);
            yield return Global.DisplayChapterTransition(transition);

            Global.SetScreenCoverColor(new Color(0, 0, 0, 1));
            yield return Global.GotoLevel();

            yield return new WaitForSeconds(2);

            Global.FadeScreenCoverColor(new Color(0, 0, 0, 0), 1);
            Global.SetMusicVolume(1);
            Global.InitLevel(areaID, stageID, 1);
            Global.HideChapterTransition();
        }
        public static IEnumerator TransitionEndToMap(NamespaceID transition, NamespaceID mapID)
        {
            Global.SetScreenCoverColor(new Color(0, 0, 0, 1));
            Global.FadeScreenCoverColor(new Color(0, 0, 0, 0), 0.5f);
            yield return Global.DisplayChapterTransition(transition, true);

            Global.FadeMusic(0, 1);
            yield return new WaitForSeconds(2);
            Global.SetMusicVolume(1);
            Global.SetScreenCoverColor(new Color(0, 0, 0, 1));
            Global.FadeScreenCoverColor(new Color(0, 0, 0, 0), 1);
            Global.HideChapterTransition();
            Global.GotoMap(mapID);
        }
    }
}
