using MVZ2.GameContent;
using MVZ2.Vanilla.Modifiers;
using PVZEngine;
using UnityEngine;

namespace MVZ2.Vanilla.Buffs
{
    [Definition(BuffNames.productionColor)]
    public class ProductionColorBuff : BuffDefinition
    {
        public ProductionColorBuff()
        {
            AddModifier(new ColorModifier(EntityProperties.COLOR_OFFSET, ModifyOperator.Average, PROP_COLOR));
        }
        public const string PROP_COLOR = "color";
    }
}
