﻿using MVZ2.Vanilla.Entities;
using PVZEngine.Buffs;
using PVZEngine.Entities;
using PVZEngine.Level;
using PVZEngine.Modifiers;

namespace MVZ2.GameContent.Buffs.Enemies
{
    [BuffDefinition(VanillaBuffNames.spiderClimb)]
    public class SpiderClimbBuff : BuffDefinition
    {
        public SpiderClimbBuff(string nsp, string name) : base(nsp, name)
        {
            AddModifier(new FloatModifier(EngineEntityProps.GRAVITY, NumberOperator.Set, 0));
            AddModifier(new BooleanModifier(VanillaEntityProps.KEEP_GROUND_FRICTION, true));
        }
    }
}
