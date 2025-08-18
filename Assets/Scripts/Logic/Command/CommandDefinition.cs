using PVZEngine.Base;

namespace MVZ2Logic.IZombie
{
    public abstract class CommandDefinition : Definition
    {
        public CommandDefinition(string nsp, string name) : base(nsp, name)
        {
        }
        public abstract void Invoke(string[] parameters);
        protected void Print(string text)
        {
            Global.Print(text);
        }
        protected void PrintLine(string text = null)
        {
            Print((text ?? string.Empty) + "\n");
        }
        public sealed override string GetDefinitionType() => LogicDefinitionTypes.COMMAND;
    }
}
