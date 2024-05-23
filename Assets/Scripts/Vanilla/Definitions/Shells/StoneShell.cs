using MVZ2.GameContent;
using PVZEngine;

namespace MVZ2.Vanilla.Buffs
{
    [Definition(ShellNames.stone)]
    public class StoneShell : ShellDefinition
    {
        public StoneShell()
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
