using MVZ2.Vanilla;
using PVZEngine.Damage;
using PVZEngine.Definitions;
using PVZEngine.Level;

namespace MVZ2.GameContent.Shells
{
    [Definition(VanillaShellNames.stone)]
    public class StoneShell : ShellDefinition
    {
        public StoneShell(string nsp, string name) : base(nsp, name)
        {
            SetProperty(VanillaShellProps.HIT_SOUND, SoundID.stone);
        }
        public override void EvaluateDamage(DamageInfo damageInfo)
        {
            base.EvaluateDamage(damageInfo);
            if (damageInfo.Effects.HasEffect(VanillaDamageEffects.PUNCH))
            {
                damageInfo.Multiply(20);
            }
        }
    }
}
