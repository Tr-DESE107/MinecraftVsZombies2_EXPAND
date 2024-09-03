using MVZ2.GameContent.Contraptions;
using MVZ2.GameContent.Modifiers;
using MVZ2.Vanilla;
using PVZEngine.Definitions;
using PVZEngine.LevelManaging;

namespace MVZ2.GameContent.Buffs
{
    [Definition(BuffNames.obsidianArmor)]
    public class ObsidianArmorBuff : BuffDefinition
    {
        public ObsidianArmorBuff(string nsp, string name) : base(nsp, name)
        {
            AddModifier(new FloatModifier(EntityProperties.MAX_HEALTH, ModifyOperator.Add, ADDED_HEALTH));
            AddModifier(new NamespaceIDModifier(ContraptionProps.FRAGMENT, ModifyOperator.Set, FragmentID.obsidianArmor));
        }
        public const float ADDED_HEALTH = 6000;
    }
}
