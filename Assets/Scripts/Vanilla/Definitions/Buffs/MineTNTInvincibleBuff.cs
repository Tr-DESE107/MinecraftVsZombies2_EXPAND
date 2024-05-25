using MVZ2.GameContent.Modifiers;
using MVZ2.Vanilla;
using PVZEngine;

namespace MVZ2.GameContent.Buffs
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
