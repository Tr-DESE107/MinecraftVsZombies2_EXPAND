using MVZ2.GameContent.Stages;
using MVZ2.Vanilla;
using PVZEngine.Definitions;
using PVZEngine.Modifiers;

namespace MVZ2.GameContent.Buffs.SeedPack
{
    [Definition(BuffNames.Level.tutorialPickaxeDisable)]
    public class TutorialPickaxeDisableBuff : BuffDefinition
    {
        public TutorialPickaxeDisableBuff(string nsp, string name) : base(nsp, name)
        {
            AddModifier(new BooleanModifier(BuiltinLevelProps.PICKAXE_DISABLED, ModifyOperator.Set, true));
        }
    }
}
