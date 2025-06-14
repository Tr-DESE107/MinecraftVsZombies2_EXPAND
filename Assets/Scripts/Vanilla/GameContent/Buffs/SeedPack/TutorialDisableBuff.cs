using MVZ2.GameContent.Seeds;
using PVZEngine.Buffs;
using PVZEngine.Level;
using PVZEngine.Modifiers;

namespace MVZ2.GameContent.Buffs.SeedPacks
{
    [BuffDefinition(VanillaBuffNames.SeedPack.tutorialDisable)]
    public class TutorialDisableBuff : BuffDefinition
    {
        public TutorialDisableBuff(string nsp, string name) : base(nsp, name)
        {
            AddModifier(new NamespaceIDModifier(EngineSeedProps.DISABLE_ID, VanillaBlueprintErrors.tutorial));
        }
    }
}
