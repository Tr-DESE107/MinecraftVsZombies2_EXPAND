using MVZ2.Vanilla;
using MVZ2.Vanilla.Level;
using PVZEngine.Buffs;
using PVZEngine.Modifiers;

namespace MVZ2.GameContent.Buffs.Level
{
    [Definition(VanillaBuffNames.Level.tutorialTriggerDisable)]
    public class TutorialTriggerDisableBuff : BuffDefinition
    {
        public TutorialTriggerDisableBuff(string nsp, string name) : base(nsp, name)
        {
            AddModifier(new BooleanModifier(VanillaLevelProps.TRIGGER_DISABLED, true));
            AddModifier(new StringModifier(VanillaLevelProps.TRIGGER_DISABLE_MESSAGE, VanillaStrings.TOOLTIP_DISABLE_MESSAGE));
        }
    }
}
