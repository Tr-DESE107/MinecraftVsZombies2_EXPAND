using MVZ2.GameContent.Modifiers;
using MVZ2.Vanilla;
using PVZEngine;

namespace MVZ2.GameContent.Buffs
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
