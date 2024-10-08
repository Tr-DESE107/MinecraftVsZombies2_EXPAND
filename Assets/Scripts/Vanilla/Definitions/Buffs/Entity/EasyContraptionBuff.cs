using MVZ2.GameContent.Contraptions;
using MVZ2.Vanilla;
using PVZEngine.Definitions;
using PVZEngine.Level;
using PVZEngine.Modifiers;
using UnityEngine;

namespace MVZ2.GameContent.Buffs.SeedPack
{
    [Definition(VanillaBuffNames.easyContraption)]
    public class EasyContraptionBuff : BuffDefinition
    {
        public EasyContraptionBuff(string nsp, string name) : base(nsp, name)
        {
            AddModifier(new FloatModifier(VanillaEntityProps.PRODUCE_SPEED, NumberOperator.Multiply, 1.5f));
            AddModifier(new Vector3Modifier(BuiltinEntityProps.LIGHT_RANGE, NumberOperator.Add, Vector3.one * 80f * 2));
        }
    }
}
