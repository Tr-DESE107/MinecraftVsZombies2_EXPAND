using MVZ2.Vanilla;
using PVZEngine.Damages;
using PVZEngine.Level;

namespace MVZ2.GameContent.Shells
{
    [Definition(VanillaShellNames.grass)]
    public class GrassShell : ShellDefinition
    {
        public GrassShell(string nsp, string name) : base(nsp, name)
        {
            SetProperty(VanillaShellProps.HIT_SOUND, SoundID.grass);
        }
        public override void EvaluateDamage(DamageInfo damageInfo)
        {
            base.EvaluateDamage(damageInfo);
            if (damageInfo.Effects.HasEffect(VanillaDamageEffects.FIRE))
            {
                damageInfo.Multiply(2);
            }
        }
    }
}
