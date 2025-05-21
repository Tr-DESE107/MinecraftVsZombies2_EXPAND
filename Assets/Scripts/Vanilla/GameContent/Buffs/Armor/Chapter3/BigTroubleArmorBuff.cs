using PVZEngine.Armors;
using PVZEngine.Buffs;
using PVZEngine.Entities;
using PVZEngine.Level;
using PVZEngine.Modifiers;

namespace MVZ2.GameContent.Buffs.Armors
{
    [BuffDefinition(VanillaBuffNames.bigTroubleArmor)]
    public class BigTroubleArmorBuff : BuffDefinition
    {
        public BigTroubleArmorBuff(string nsp, string name) : base(nsp, name)
        {
            AddModifier(new FloatModifier(EngineArmorProps.MAX_HEALTH, NumberOperator.Multiply, 4));
        }
    }
}
