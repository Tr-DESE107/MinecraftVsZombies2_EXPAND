using MVZ2.GameContent.Contraptions;
using MVZ2.Vanilla;
using PVZEngine.Definitions;
using PVZEngine.Level;
using PVZEngine.Modifiers;

namespace MVZ2.GameContent.Buffs
{
    [Definition(VanillaBuffNames.obsidianArmor)]
    public class ObsidianArmorBuff : BuffDefinition
    {
        public ObsidianArmorBuff(string nsp, string name) : base(nsp, name)
        {
            AddModifier(new FloatModifier(EngineEntityProps.MAX_HEALTH, ModifyOperator.Multiply, HEALTH_MULTIPLIER));
            AddModifier(new NamespaceIDModifier(VanillaContraptionProps.FRAGMENT_ID, ModifyOperator.Set, VanillaFragmentID.obsidianArmor));
        }
        public const float HEALTH_MULTIPLIER = 2.5f;
    }
}
