using MVZ2.Vanilla;
using PVZEngine.Buffs;
using PVZEngine.Entities;
using PVZEngine.Modifiers;

namespace MVZ2.GameContent.Buffs.Contraptions
{
    [Definition(VanillaBuffNames.mineTNTInvincible)]
    public class MagichestInvincibleBuff : BuffDefinition
    {
        public MagichestInvincibleBuff(string nsp, string name) : base(nsp, name)
        {
            AddModifier(new BooleanModifier(EngineEntityProps.INVINCIBLE, true));
        }
    }
}
