using PVZEngine.LevelManagement;

namespace PVZEngine.Definitions
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
