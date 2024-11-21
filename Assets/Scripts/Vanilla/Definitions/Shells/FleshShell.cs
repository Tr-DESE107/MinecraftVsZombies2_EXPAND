using MVZ2.Vanilla;
using PVZEngine.Damage;
using PVZEngine.Definitions;
using PVZEngine.Level;

namespace MVZ2.GameContent.Shells
{
    [Definition(VanillaShellNames.flesh)]
    public class FleshShell : ShellDefinition
    {
        public FleshShell(string nsp, string name) : base(nsp, name)
        {
            SetProperty(VanillaShellProps.SLICE_CRITICAL, true);
            SetProperty(VanillaShellProps.SLICE_BLOOD, true);
            SetProperty(VanillaShellProps.HIT_SOUND, SoundID.splat);
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
