using MVZ2.Vanilla;
using MVZ2.Vanilla.Level;
using PVZEngine.Buffs;
using PVZEngine.Modifiers;

namespace MVZ2.GameContent.Buffs.Level
{
    [Definition(VanillaBuffNames.Level.tutorialPickaxeDisable)]
    public class TutorialPickaxeDisableBuff : BuffDefinition
    {
        public TutorialPickaxeDisableBuff(string nsp, string name) : base(nsp, name)
        {
            AddModifier(new BooleanModifier(VanillaLevelProps.PICKAXE_DISABLED, true));
            AddModifier(new StringModifier(VanillaLevelProps.PICKAXE_DISABLE_MESSAGE, VanillaStrings.TOOLTIP_DISABLE_MESSAGE));
        }
    }
}
