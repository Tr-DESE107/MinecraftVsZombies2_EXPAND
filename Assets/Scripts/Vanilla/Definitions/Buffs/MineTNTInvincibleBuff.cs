using MVZ2.GameContent;
using MVZ2.Vanilla.Modifiers;
using PVZEngine;
using UnityEngine;

namespace MVZ2.Vanilla.Buffs
{
    [Definition(BuffNames.mineTNTInvincible)]
    public class MineTNTInvincibleBuff : BuffDefinition
    {
        public MineTNTInvincibleBuff()
        {
            AddModifier(new BooleanModifier(EntityProperties.INVINCIBLE, ModifyOperator.Set, true));
        }
    }
}
