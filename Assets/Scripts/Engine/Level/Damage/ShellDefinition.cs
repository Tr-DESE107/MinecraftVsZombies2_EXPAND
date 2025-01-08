using PVZEngine.Base;
using PVZEngine.Definitions;

namespace PVZEngine.Damages
{
    public abstract class ShellDefinition : Definition
    {
        public ShellDefinition(string nsp, string name) : base(nsp, name)
        {
        }
        public virtual void EvaluateDamage(DamageInput damageInfo)
        {
        }
        public sealed override string GetDefinitionType() => EngineDefinitionTypes.SHELL;
    }
}
