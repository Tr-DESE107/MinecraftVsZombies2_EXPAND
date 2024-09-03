using MVZ2.GameContent.Modifiers;
using MVZ2.Vanilla;
using PVZEngine.Definitions;
using PVZEngine.LevelManaging;
using UnityEngine;

namespace MVZ2.GameContent.Buffs
{
    [Definition(BuffNames.damageColor)]
    public class DamageColorBuff : BuffDefinition
    {
        public DamageColorBuff(string nsp, string name) : base(nsp, name)
        {
            AddModifier(new ColorModifier(EntityProperties.COLOR_OFFSET, ModifyOperator.Average, new Color(1, 0, 0, 0)));
        }
    }
}
