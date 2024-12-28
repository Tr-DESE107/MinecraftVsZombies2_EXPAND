using MVZ2.Vanilla;
using MVZ2.Vanilla.Entities;
using PVZEngine.Buffs;
using PVZEngine.Entities;
using PVZEngine.Modifiers;
using UnityEngine;

namespace MVZ2.GameContent.Buffs.Enemies
{
    [Definition(VanillaBuffNames.spiderClimb)]
    public class SpiderClimbBuff : BuffDefinition
    {
        public SpiderClimbBuff(string nsp, string name) : base(nsp, name)
        {
            AddModifier(new FloatModifier(EngineEntityProps.GRAVITY, NumberOperator.Multiply, 0));
            AddModifier(new BooleanModifier(VanillaEntityProps.KEEP_GROUND_FRICTION, true));
        }
    }
}
