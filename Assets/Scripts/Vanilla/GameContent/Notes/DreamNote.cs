using MVZ2Logic;
using MVZ2Logic.Notes;

namespace MVZ2.GameContent.Notes
{
    [NoteDefinition(VanillaNoteNames.dream)]
    public class DreamNote : NoteDefinition
    {
        public DreamNote(string nsp, string name) : base(nsp, name)
        {
        }
        public override void OnBack(INote note)
        {
            base.OnBack(note);
            Global.GotoMainmenuOrMap();
        }
    }
}
