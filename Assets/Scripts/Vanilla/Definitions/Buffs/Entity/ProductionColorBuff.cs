using MVZ2.Vanilla;
using PVZEngine.Definitions;
using PVZEngine.Modifiers;

namespace MVZ2.GameContent.Buffs
{
    [Definition(BuffNames.productionColor)]
    public class ProductionColorBuff : BuffDefinition
    {
        public ProductionColorBuff(string nsp, string name) : base(nsp, name)
        {
            AddModifier(new ColorModifier(EntityProperties.COLOR_OFFSET, ModifyOperator.Average, PROP_COLOR));
        }
        public const string PROP_COLOR = "color";
    }
}
