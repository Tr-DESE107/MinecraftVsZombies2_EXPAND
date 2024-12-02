using PVZEngine.Base;

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
    }
}
