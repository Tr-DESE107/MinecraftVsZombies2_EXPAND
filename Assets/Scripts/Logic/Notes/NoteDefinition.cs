using PVZEngine.Base;

namespace MVZ2Logic.Notes
{
    public abstract class NoteDefinition : Definition
    {
        public NoteDefinition(string nsp, string name) : base(nsp, name)
        {
        }
        public virtual void OnBack(INote note) { }
        public sealed override string GetDefinitionType() => LogicDefinitionTypes.NOTE;
    }
}
