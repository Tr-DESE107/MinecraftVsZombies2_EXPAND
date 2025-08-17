using System;
using PVZEngine.Base;

namespace MVZ2Logic.IZombie
{
    public abstract class CommandDefinition : Definition
    {
        public CommandDefinition(string nsp, string name) : base(nsp, name)
        {
        }
        public void Invoke(string[] parameters)
        {
            ValidateParameters(parameters);
            Execute(parameters);
        }
        protected virtual void ValidateParameters(string[] parameters)
        {
            throw new NotImplementedException();
        }
        protected abstract void Execute(string[] parameters);
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
