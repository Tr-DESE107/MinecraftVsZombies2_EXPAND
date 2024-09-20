using PVZEngine.Base;
using PVZEngine.Level;

namespace PVZEngine.Definitions
{
    public abstract class TalkEndDefinition : Definition
    {
        public TalkEndDefinition(string nsp, string name) : base(nsp, name)
        {
        }
        public virtual void Execute(LevelEngine level) { }
    }
}
