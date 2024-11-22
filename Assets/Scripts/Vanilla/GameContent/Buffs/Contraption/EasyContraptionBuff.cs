using MVZ2.Vanilla;
using MVZ2.Vanilla.Entities;
using PVZEngine.Buffs;
using PVZEngine.Modifiers;
using UnityEngine;

namespace MVZ2.GameContent.Buffs.SeedPacks
{
    [Definition(VanillaBuffNames.easyContraption)]
    public class EasyContraptionBuff : BuffDefinition
    {
        public EasyContraptionBuff(string nsp, string name) : base(nsp, name)
        {
            AddModifier(new FloatModifier(VanillaEntityProps.PRODUCE_SPEED, NumberOperator.Multiply, 1.5f));
            AddModifier(new Vector3Modifier(VanillaEntityProps.LIGHT_RANGE, NumberOperator.Add, Vector3.one * 80f * 2));
        }
    }
}
