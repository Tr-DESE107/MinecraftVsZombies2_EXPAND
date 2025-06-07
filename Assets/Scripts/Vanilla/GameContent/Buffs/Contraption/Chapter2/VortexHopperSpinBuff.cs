using MVZ2.Vanilla.Entities;
using PVZEngine.Buffs;
using PVZEngine.Entities;
using PVZEngine.Level;
using PVZEngine.Modifiers;

namespace MVZ2.GameContent.Buffs.Contraptions
{
    [BuffDefinition(VanillaBuffNames.vortexHopperSpin)]
    public class VortexHopperSpinBuff : BuffDefinition
    {
        public VortexHopperSpinBuff(string nsp, string name) : base(nsp, name)
        {
            AddModifier(new BooleanModifier(EngineEntityProps.INVINCIBLE, true));
            AddModifier(new BooleanModifier(VanillaEntityProps.CAN_DEACTIVE, false));
            AddModifier(new FloatModifier(EngineEntityProps.GRAVITY, NumberOperator.Multiply, 0));
        }
    }
}
