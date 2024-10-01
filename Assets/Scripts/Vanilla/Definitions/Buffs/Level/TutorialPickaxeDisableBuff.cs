using MVZ2.Vanilla;
using PVZEngine.Definitions;
using PVZEngine.Modifiers;

namespace MVZ2.GameContent.Buffs.SeedPack
{
    [Definition(VanillaBuffNames.Level.tutorialPickaxeDisable)]
    public class TutorialPickaxeDisableBuff : BuffDefinition
    {
        public TutorialPickaxeDisableBuff(string nsp, string name) : base(nsp, name)
        {
            AddModifier(new BooleanModifier(BuiltinLevelProps.PICKAXE_DISABLED, ModifyOperator.Set, true));
            AddModifier(new StringModifier(BuiltinLevelProps.PICKAXE_DISABLE_MESSAGE, ModifyOperator.Set, VanillaStrings.STRING_DISABLE_MESSAGE));
        }
    }
}
