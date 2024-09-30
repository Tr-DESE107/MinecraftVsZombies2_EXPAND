using MVZ2.ChapterTransition;
using MVZ2.Definitions;
using MVZ2.GameContent;
using MVZ2.Note;

namespace MVZ2.Vanilla
{
    [Definition(VanillaNoteNames.halloween)]
    public class HalloweenNote : NoteDefinition
    {
        public HalloweenNote(string nsp, string name) : base(nsp, name)
        {
        }
        public override void OnBack(INote note)
        {
            base.OnBack(note);
            Global.GotoMainmenuOrMap();
        }
    }
}
