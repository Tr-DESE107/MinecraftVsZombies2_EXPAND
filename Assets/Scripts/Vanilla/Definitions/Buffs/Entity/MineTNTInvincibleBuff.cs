using MVZ2.Vanilla;
using PVZEngine.Definitions;
using PVZEngine.Modifiers;

namespace MVZ2.GameContent.Buffs
{
    [Definition(BuffNames.mineTNTInvincible)]
    public class MineTNTInvincibleBuff : BuffDefinition
    {
        public MineTNTInvincibleBuff(string nsp, string name) : base(nsp, name)
        {
            AddModifier(new BooleanModifier(EntityProperties.INVINCIBLE, ModifyOperator.Set, true));
        }
    }
}
