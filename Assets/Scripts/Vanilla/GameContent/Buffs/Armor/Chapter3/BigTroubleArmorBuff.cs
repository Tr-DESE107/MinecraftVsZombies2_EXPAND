using MVZ2.Vanilla;
using MVZ2.Vanilla.Entities;
using PVZEngine.Buffs;
using PVZEngine.Entities;
using PVZEngine.Modifiers;
using UnityEngine;

namespace MVZ2.GameContent.Buffs.Armors
{
    [Definition(VanillaBuffNames.bigTroubleArmor)]
    public class BigTroubleArmorBuff : BuffDefinition
    {
        public BigTroubleArmorBuff(string nsp, string name) : base(nsp, name)
        {
            AddModifier(new FloatModifier(EngineEntityProps.MAX_HEALTH, NumberOperator.Multiply, 4));
        }
    }
}
