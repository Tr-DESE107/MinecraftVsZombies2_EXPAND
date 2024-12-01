using MVZ2.GameContent.Damages;
using MVZ2.Vanilla;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Shells;
using PVZEngine.Damages;

namespace MVZ2.GameContent.Shells
{
    [Definition(VanillaShellNames.flesh)]
    public class FleshShell : ShellDefinition
    {
        public FleshShell(string nsp, string name) : base(nsp, name)
        {
            SetProperty(VanillaShellProps.SLICE_CRITICAL, true);
            SetProperty(VanillaShellProps.HIT_SOUND, VanillaSoundID.splat);
        }
        public override void EvaluateDamage(DamageInfo damageInfo)
        {
            base.EvaluateDamage(damageInfo);
            if (damageInfo.Effects.HasEffect(VanillaDamageEffects.SLICE))
            {
                damageInfo.Multiply(2);
            }
        }
    }
}
