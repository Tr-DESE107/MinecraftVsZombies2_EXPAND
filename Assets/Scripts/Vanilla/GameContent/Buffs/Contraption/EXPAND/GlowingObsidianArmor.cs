using MVZ2.GameContent.Fragments;
using MVZ2.Vanilla.Contraptions;
using PVZEngine.Buffs;
using PVZEngine.Level;
using PVZEngine.Modifiers;

namespace MVZ2.GameContent.Buffs.Contraptions
{
    [BuffDefinition(VanillaBuffNames.GlowingObsidianArmorBuff)]
    public class GlowingObsidianArmorBuff : BuffDefinition
    {
        public GlowingObsidianArmorBuff(string nsp, string name) : base(nsp, name)
        {
            AddModifier(new MaxHealthModifier(NumberOperator.Multiply, HEALTH_MULTIPLIER));
            AddModifier(new NamespaceIDModifier(VanillaContraptionProps.FRAGMENT_ID, VanillaFragmentID.GlowingObsidianArmor));
        }
        public const float HEALTH_MULTIPLIER = 2.5f;
    }
}
