using MVZ2.GameContent;
using MVZ2.Vanilla;
using PVZEngine;

namespace MVZ2.GameContent.Shells
{
    [Definition(ShellNames.stone)]
    public class StoneShell : ShellDefinition
    {
        public StoneShell(string nsp, string name) : base(nsp, name)
        {
            SetProperty(ShellProps.HIT_SOUND, SoundID.stone);
        }
        public override void EvaluateDamage(DamageInfo damageInfo)
        {
            base.EvaluateDamage(damageInfo);
            if (damageInfo.Effects.HasEffect(DamageEffects.PUNCH))
            {
                damageInfo.Multiply(20);
            }
        }
    }
}
