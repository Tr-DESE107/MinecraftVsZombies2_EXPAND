using MVZ2.Vanilla.Properties;
using PVZEngine.Buffs;
using PVZEngine.Entities;
using PVZEngine.Level;
using PVZEngine.Modifiers;
using UnityEngine;

namespace MVZ2.GameContent.Buffs.Contraptions
{
    [BuffDefinition(VanillaBuffNames.productionColor)]
    public class ProductionColorBuff : BuffDefinition
    {
        public ProductionColorBuff(string nsp, string name) : base(nsp, name)
        {
            AddModifier(new ColorModifier(EngineEntityProps.COLOR_OFFSET, PROP_COLOR));
        }
        public static readonly VanillaBuffPropertyMeta<Color> PROP_COLOR = new VanillaBuffPropertyMeta<Color>("color");
    }
}
