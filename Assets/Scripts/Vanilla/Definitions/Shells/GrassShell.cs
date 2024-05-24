using MVZ2.GameContent;
using PVZEngine;

namespace MVZ2.Vanilla.Buffs
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
