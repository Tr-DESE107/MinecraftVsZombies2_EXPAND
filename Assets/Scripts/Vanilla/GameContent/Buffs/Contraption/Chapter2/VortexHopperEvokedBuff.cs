using MVZ2.Vanilla.Entities;
using PVZEngine.Buffs;
using PVZEngine.Level;
using PVZEngine.Modifiers;

namespace MVZ2.GameContent.Buffs.Contraptions
{
    [BuffDefinition(VanillaBuffNames.vortexHopperEvoked)]
    public class VortexHopperEvokedBuff : BuffDefinition
    {
        public VortexHopperEvokedBuff(string nsp, string name) : base(nsp, name)
        {
            AddModifier(new FloatModifier(VanillaEntityProps.RANGE, NumberOperator.Multiply, 3));
        }
    }
}
