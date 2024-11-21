using System.Collections;
using MVZ2.GameContent;
using MVZ2Logic;
using MVZ2Logic.Notes;
using UnityEngine;

namespace MVZ2.Vanilla
{
    [Definition(VanillaNoteNames.prologue)]
    public class PrologueNote : NoteDefinition
    {
        public PrologueNote(string nsp, string name) : base(nsp, name)
        {
        }
        public override void OnBack(INote note)
        {
            base.OnBack(note);
            note.SetInteractable(false);
            Global.StartCoroutine(NoteCoroutine());
        }
        private IEnumerator NoteCoroutine()
        {
            Global.FadeMusic(0, 2);
            Global.SetBlackScreen(0);
            Global.FadeBlackScreen(1, 1);

            yield return new WaitForSeconds(2);

            Global.SetBlackScreen(0);
            yield return Global.DisplayChapterTransition(ChapterTransitionID.halloween);
            yield return Global.GotoLevel();

            yield return new WaitForSeconds(2);

            Global.SetBlackScreen(1);
            Global.FadeBlackScreen(0, 1);
            Global.SetMusicVolume(1);
            Global.InitLevel(VanillaAreaID.halloween, VanillaStageID.halloween1, 1);
            Global.HideChapterTransition();
        }
    }
}
