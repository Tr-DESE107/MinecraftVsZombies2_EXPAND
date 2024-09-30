using MVZ2.Definitions;
using MVZ2.GameContent;
using MVZ2.Note;

namespace MVZ2.Vanilla
{
    [Definition(BuiltinNoteNames.help)]
    public class HelpNote : NoteDefinition
    {
        public HelpNote(string nsp, string name) : base(nsp, name)
        {
        }
        public override void OnBack(INote note)
        {
            base.OnBack(note);
            Global.GotoMainmenu();
        }
    }
}
