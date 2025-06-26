﻿using MVZ2.GameContent.Damages;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Shells;
using PVZEngine.Damages;
using PVZEngine.Level;

namespace MVZ2.GameContent.Shells
{
    [ShellDefinition(VanillaShellNames.netherrack)]
    public class NetherrackShell : ShellDefinition
    {
        public NetherrackShell(string nsp, string name) : base(nsp, name)
        {
            SetProperty(VanillaShellProps.HIT_SOUND, VanillaSoundID.netherrackBreak);
            SetProperty(VanillaShellProps.BLOCKS_SLICE, true);
            SetProperty(VanillaShellProps.BLOCKS_FIRE, true);
        }
        public override void EvaluateDamage(DamageInput damageInfo)
        {
            base.EvaluateDamage(damageInfo);
            if (damageInfo.Effects.HasEffect(VanillaDamageEffects.FIRE))
            {
                damageInfo.Multiply(0);
            }
            if (damageInfo.Effects.HasEffect(VanillaDamageEffects.PUNCH))
            {
                damageInfo.Multiply(20);
            }
        }
    }
}
