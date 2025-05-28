﻿using MVZ2.Vanilla.Level;
using PVZEngine.Buffs;
using PVZEngine.Level;
using PVZEngine.Modifiers;

namespace MVZ2.GameContent.Buffs.Level
{
    [BuffDefinition(VanillaBuffNames.Level.greedyVacuum)]
    public class GreedyVacuumBuff : BuffDefinition
    {
        public GreedyVacuumBuff(string nsp, string name) : base(nsp, name)
        {
            AddModifier(new BooleanModifier(VanillaStageProps.AUTO_COLLECT_ENERGY, true));
            AddModifier(new BooleanModifier(VanillaStageProps.AUTO_COLLECT_STARSHARD, true));
        }
    }
}
