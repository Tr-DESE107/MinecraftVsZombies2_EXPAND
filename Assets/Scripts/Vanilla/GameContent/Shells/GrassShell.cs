using MVZ2.GameContent.Damages;
using MVZ2.Vanilla;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Shells;
using PVZEngine.Damages;

namespace MVZ2.GameContent.Shells
{
    [Definition(VanillaShellNames.grass)]
    public class GrassShell : ShellDefinition
    {
        public GrassShell(string nsp, string name) : base(nsp, name)
        {
            SetProperty(VanillaShellProps.HIT_SOUND, VanillaSoundID.grass);
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
