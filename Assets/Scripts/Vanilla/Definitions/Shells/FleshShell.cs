using MVZ2.GameContent;
using PVZEngine;

namespace MVZ2.Vanilla.Buffs
{
    [Definition(ShellNames.flesh)]
    public class FleshShell : ShellDefinition
    {
        public FleshShell()
        {
            SetProperty(ShellProps.SLICE_CRITICAL, true);
            SetProperty(ShellProps.SLICE_BLOOD, true);
            SetProperty(ShellProps.HIT_SOUND, SoundID.splat);
        }
        public override void EvaluateDamage(DamageInfo damageInfo)
        {
            base.EvaluateDamage(damageInfo);
            if (damageInfo.Effects.HasEffect(DamageEffects.SLICE))
            {
                damageInfo.Multiply(2);
            }
        }
    }
}
