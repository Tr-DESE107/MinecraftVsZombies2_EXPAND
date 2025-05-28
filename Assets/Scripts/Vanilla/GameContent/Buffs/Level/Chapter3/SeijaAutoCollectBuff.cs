﻿using MVZ2.Vanilla.Level;
using MVZ2Logic.Level;
using PVZEngine.Buffs;
using PVZEngine.Level;
using PVZEngine.Modifiers;

namespace MVZ2.GameContent.Buffs.Level
{
    [BuffDefinition(VanillaBuffNames.Level.seijaAutoCollect)]
    public class SeijaAutoCollectBuff : BuffDefinition
    {
        public SeijaAutoCollectBuff(string nsp, string name) : base(nsp, name)
        {
            AddModifier(new BooleanModifier(VanillaStageProps.AUTO_COLLECT_ALL, true));
            AddModifier(new BooleanModifier(VanillaStageProps.NO_ENERGY, true));
            AddModifier(new BooleanModifier(LogicLevelProps.PAUSE_DISABLED, true));
        }
    }
}
