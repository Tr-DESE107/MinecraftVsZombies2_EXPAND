using MVZ2.Vanilla;
using PVZEngine.Buffs;
using PVZEngine.Level;
using PVZEngine.Modifiers;

namespace MVZ2.GameContent.Buffs.SeedPack
{
    [Definition(VanillaBuffNames.SeedPack.tutorialDisable)]
    public class TutorialDisableBuff : BuffDefinition
    {
        public TutorialDisableBuff(string nsp, string name) : base(nsp, name)
        {
            AddModifier(new BooleanModifier(PVZEngine.Level.EngineSeedProps.DISABLED, true));
            AddModifier(new StringModifier(PVZEngine.Level.EngineSeedProps.DISABLE_MESSAGE, VanillaStrings.TOOLTIP_DISABLE_MESSAGE));
        }
    }
}
