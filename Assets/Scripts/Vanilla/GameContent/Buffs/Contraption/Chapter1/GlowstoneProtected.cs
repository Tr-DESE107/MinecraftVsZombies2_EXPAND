﻿using MVZ2.Vanilla.Entities;
using PVZEngine.Buffs;
using PVZEngine.Level;
using PVZEngine.Modifiers;

namespace MVZ2.GameContent.Buffs.Contraptions
{
    [BuffDefinition(VanillaBuffNames.glowstoneProtected)]
    public class GlowstoneProtected : BuffDefinition
    {
        public GlowstoneProtected(string nsp, string name) : base(nsp, name)
        {
            AddModifier(new BooleanModifier(VanillaEntityProps.LOYAL, true));
        }
    }
}
