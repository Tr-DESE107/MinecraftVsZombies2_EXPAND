using MVZ2.Note;
using PVZEngine.Base;

namespace MVZ2.Definitions
{
    public abstract class NoteDefinition : Definition
    {
        public NoteDefinition(string nsp, string name) : base(nsp, name)
        {
        }
        public virtual void OnBack(INote note) { }
    }
}
