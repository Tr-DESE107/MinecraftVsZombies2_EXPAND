﻿using MVZ2Logic;
using MVZ2Logic.Notes;

namespace MVZ2.GameContent.Notes
{
    [NoteDefinition(VanillaNoteNames.help)]
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
