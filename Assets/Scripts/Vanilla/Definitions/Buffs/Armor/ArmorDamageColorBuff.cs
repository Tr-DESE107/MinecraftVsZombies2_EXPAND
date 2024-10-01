using MVZ2.Vanilla;
using PVZEngine.Definitions;
using PVZEngine.Level;
using PVZEngine.Modifiers;
using UnityEngine;

namespace MVZ2.GameContent.Buffs
{
    [Definition(VanillaBuffNames.armorDamageColor)]
    public class ArmorDamageColorBuff : BuffDefinition
    {
        public ArmorDamageColorBuff(string nsp, string name) : base(nsp, name)
        {
            AddModifier(new ColorModifier(EngineArmorProps.COLOR_OFFSET, ModifyOperator.Average, new Color(1, 0, 0, 0)));
        }
        public override void PostUpdate(Buff buff)
        {
            base.PostUpdate(buff);
            buff.Target.RemoveBuff(buff);
        }
    }
}
