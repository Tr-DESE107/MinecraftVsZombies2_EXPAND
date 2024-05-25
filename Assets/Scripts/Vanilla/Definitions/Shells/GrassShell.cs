using MVZ2.GameContent;
using MVZ2.Vanilla;
using PVZEngine;

namespace MVZ2.GameContent.Shells
{
    [Definition(ShellNames.grass)]
    public class GrassShell : ShellDefinition
    {
        public GrassShell()
        {
            SetProperty(ShellProps.HIT_SOUND, SoundID.grass);
        }
        public override void EvaluateDamage(DamageInfo damageInfo)
        {
            base.EvaluateDamage(damageInfo);
            if (damageInfo.Effects.HasEffect(DamageEffects.FIRE))
            {
                damageInfo.Multiply(2);
            }
        }
    }
}
