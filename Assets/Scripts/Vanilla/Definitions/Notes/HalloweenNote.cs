using MVZ2.GameContent;
using MVZ2Logic;
using MVZ2Logic.Notes;

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
