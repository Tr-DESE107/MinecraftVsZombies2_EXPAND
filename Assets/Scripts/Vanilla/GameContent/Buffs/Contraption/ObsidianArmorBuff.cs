using MVZ2.GameContent.Fragments;
using MVZ2.Vanilla;
using MVZ2.Vanilla.Contraptions;
using PVZEngine.Buffs;
using PVZEngine.Entities;
using PVZEngine.Modifiers;

namespace MVZ2.GameContent.Buffs.Contraptions
{
    [Definition(VanillaBuffNames.obsidianArmor)]
    public class ObsidianArmorBuff : BuffDefinition
    {
        public ObsidianArmorBuff(string nsp, string name) : base(nsp, name)
        {
            AddModifier(new FloatModifier(EngineEntityProps.MAX_HEALTH, NumberOperator.Multiply, HEALTH_MULTIPLIER));
            AddModifier(new NamespaceIDModifier(VanillaContraptionProps.FRAGMENT_ID, VanillaFragmentID.obsidianArmor));
        }
        public const float HEALTH_MULTIPLIER = 2.5f;
    }
}
