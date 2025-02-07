using MVZ2.GameContent.Damages;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Shells;
using PVZEngine.Damages;
using PVZEngine.Level;

namespace MVZ2.GameContent.Shells
{
    [ShellDefinition(VanillaShellNames.stone)]
    public class StoneShell : ShellDefinition
    {
        public StoneShell(string nsp, string name) : base(nsp, name)
        {
            SetProperty(VanillaShellProps.HIT_SOUND, VanillaSoundID.stone);
            SetProperty(VanillaShellProps.BLOCKS_SLICE, true);
        }
        public override void EvaluateDamage(DamageInput damageInfo)
        {
            base.EvaluateDamage(damageInfo);
            if (damageInfo.Effects.HasEffect(VanillaDamageEffects.PUNCH))
            {
                damageInfo.Multiply(20);
            }
        }
    }
}
