﻿using MVZ2.Vanilla.Entities;
using PVZEngine.Buffs;
using PVZEngine.Entities;
using PVZEngine.Level;
using PVZEngine.Modifiers;

namespace MVZ2.GameContent.Buffs.Projectiles
{
    [BuffDefinition(VanillaBuffNames.Projectile.invertedMirror)]
    public class InvertedMirrorBuff : BuffDefinition
    {
        public InvertedMirrorBuff(string nsp, string name) : base(nsp, name)
        {
            AddModifier(new IntModifier(EngineEntityProps.FACTION, NumberOperator.Set, VanillaFactions.NEUTRAL));
        }
    }
}
