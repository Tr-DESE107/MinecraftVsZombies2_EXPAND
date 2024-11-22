using MVZ2.Vanilla;
using PVZEngine.Buffs;
using PVZEngine.Entities;
using PVZEngine.Modifiers;

namespace MVZ2.GameContent.Buffs.Contraptions
{
    [Definition(VanillaBuffNames.productionColor)]
    public class ProductionColorBuff : BuffDefinition
    {
        public ProductionColorBuff(string nsp, string name) : base(nsp, name)
        {
            AddModifier(new ColorModifier(EngineEntityProps.COLOR_OFFSET, PROP_COLOR));
        }
        public const string PROP_COLOR = "color";
    }
}
