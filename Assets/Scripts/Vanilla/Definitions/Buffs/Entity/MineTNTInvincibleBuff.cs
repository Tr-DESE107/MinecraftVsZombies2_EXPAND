using MVZ2.Vanilla;
using PVZEngine.Definitions;
using PVZEngine.Level;
using PVZEngine.Modifiers;

namespace MVZ2.GameContent.Buffs
{
    [Definition(VanillaBuffNames.mineTNTInvincible)]
    public class MineTNTInvincibleBuff : BuffDefinition
    {
        public MineTNTInvincibleBuff(string nsp, string name) : base(nsp, name)
        {
            AddModifier(new BooleanModifier(EngineEntityProps.INVINCIBLE, true));
        }
    }
}
