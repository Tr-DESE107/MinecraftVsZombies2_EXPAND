using PVZEngine.Buffs;
using PVZEngine.Level;
using PVZEngine.Modifiers;

namespace MVZ2.GameContent.Buffs.Armors
{
    [BuffDefinition(VanillaBuffNames.FrankensteinBrainAddHealthBuff)]
    public class FrankensteinBrainAddHealthBuff : BuffDefinition
    {
        public FrankensteinBrainAddHealthBuff(string nsp, string name) : base(nsp, name)
        {
            AddModifier(new MaxHealthModifier(NumberOperator.Multiply, 1.25f));
        }
    }
}
