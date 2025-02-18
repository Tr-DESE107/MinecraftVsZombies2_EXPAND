using MVZ2Logic;
using MVZ2Logic.Notes;

namespace MVZ2.GameContent.Notes
{
    [NoteDefinition(VanillaNoteNames.castle)]
    public class CastleNote : NoteDefinition
    {
        public CastleNote(string nsp, string name) : base(nsp, name)
        {
        }
        public override void OnBack(INote note)
        {
            base.OnBack(note);
            Global.GotoMainmenuOrMap();
        }
    }
}
