using System.Collections.Generic;

namespace PVZEngine
{
    public abstract class ShellDefinition : Definition
    {
        public ShellDefinition(string nsp, string name) : base(nsp, name)
        {
        }
        public virtual void EvaluateDamage(DamageInfo damageInfo)
        {
        }
    }
}
