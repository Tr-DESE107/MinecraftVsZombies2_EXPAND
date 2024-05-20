using MVZ2.Vanilla.Modifiers;
using PVZEngine;
using UnityEngine;

namespace MVZ2.Vanilla.Buffs
{
    public class DamageColorBuff : BuffDefinition
    {
        public DamageColorBuff()
        {
            AddModifier(new ColorModifier(EntityProperties.COLOR_OFFSET, ModifyOperator.Average, new Color(1, 0, 0, 0)));
        }
    }
}
