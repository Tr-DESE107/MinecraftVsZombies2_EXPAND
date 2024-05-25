using MVZ2.GameContent;
using MVZ2.Vanilla;
using PVZEngine;

namespace MVZ2.GameContent.Shells
{
    [Definition(ShellNames.flesh)]
    public class FleshShell : ShellDefinition
    {
        public FleshShell(string nsp, string name) : base(nsp, name)
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
