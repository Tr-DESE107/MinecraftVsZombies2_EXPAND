using MVZ2.Vanilla.Entities;
using PVZEngine.Buffs;
using PVZEngine.Level;
using PVZEngine.Modifiers;
using UnityEngine;

namespace MVZ2.GameContent.Buffs.Contraptions
{
    [BuffDefinition(VanillaBuffNames.hfpdUpgraded)]
    public class HFPDUpgradedBuff : BuffDefinition
    {
        public HFPDUpgradedBuff(string nsp, string name) : base(nsp, name)
        {
            AddModifier(new BooleanModifier(VanillaEntityProps.IS_LIGHT_SOURCE, true));
            AddModifier(new Vector3Modifier(VanillaEntityProps.LIGHT_RANGE, NumberOperator.Add, LIGHT_RANGE_ADDITION));
        }
        public static readonly Vector3 LIGHT_RANGE_ADDITION = new Vector3(80, 80, 80);
    }
}
