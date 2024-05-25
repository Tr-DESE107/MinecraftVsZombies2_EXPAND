using MVZ2.GameContent.Modifiers;
using MVZ2.Vanilla;
using PVZEngine;
using UnityEngine;

namespace MVZ2.GameContent.Buffs
{
    [Definition(BuffNames.armorDamageColor)]
    public class ArmorDamageColorBuff : BuffDefinition
    {
        public ArmorDamageColorBuff()
        {
            AddModifier(new ColorModifier(ArmorProperties.COLOR_OFFSET, ModifyOperator.Average, new Color(1, 0, 0, 0)));
        }
    }
}
