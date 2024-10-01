using MVZ2.GameContent;
using MVZ2.Vanilla;
using PVZEngine.Definitions;
using PVZEngine.Level;
using PVZEngine.Modifiers;
using UnityEngine;

namespace MVZ2.Vanilla.Buffs
{
    [Definition(VanillaBuffNames.starshardCarrier)]
    public class StarshardCarrierBuff : BuffDefinition
    {
        public StarshardCarrierBuff(string nsp, string name) : base(nsp, name)
        {
            AddModifier(new ColorModifier(EngineEntityProps.COLOR_OFFSET, ModifyOperator.Average, new Color(0, 0.5f, 0, 0)));
        }
    }
}
