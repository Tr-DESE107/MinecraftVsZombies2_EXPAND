using MVZ2.GameContent.Seeds;
using MVZ2.Vanilla.Level;
using PVZEngine.Buffs;
using PVZEngine.Level;
using PVZEngine.Modifiers;

namespace MVZ2.GameContent.Buffs.Level
{
    [BuffDefinition(VanillaBuffNames.Level.tutorialPickaxeDisable)]
    public class TutorialPickaxeDisableBuff : BuffDefinition
    {
        public TutorialPickaxeDisableBuff(string nsp, string name) : base(nsp, name)
        {
            AddModifier(new NamespaceIDModifier(VanillaLevelProps.PICKAXE_DISABLE_ID, VanillaBlueprintErrors.tutorial));
        }
    }
}
