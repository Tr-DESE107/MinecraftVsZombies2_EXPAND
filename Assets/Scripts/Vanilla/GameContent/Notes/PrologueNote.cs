using MVZ2.GameContent.Areas;
using MVZ2.GameContent.Stages;
using MVZ2.Vanilla;
using MVZ2Logic;
using MVZ2Logic.Notes;

namespace MVZ2.GameContent.Notes
{
    [NoteDefinition(VanillaNoteNames.prologue)]
    public class PrologueNote : NoteDefinition
    {
        public PrologueNote(string nsp, string name) : base(nsp, name)
        {
        }
        public override void OnBack(INote note)
        {
            base.OnBack(note);
            note.SetInteractable(false);
            Global.StartCoroutine(VanillaChapterTransitions.TransitionToLevel(VanillaChapterTransitions.halloween, VanillaAreaID.halloween, VanillaStageID.halloween1));
        }
    }
}
