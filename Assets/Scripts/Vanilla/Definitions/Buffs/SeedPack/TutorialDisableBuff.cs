using MukioI18n;
using MVZ2.Vanilla;
using PVZEngine.Definitions;
using PVZEngine.Modifiers;

namespace MVZ2.GameContent.Buffs.SeedPack
{
    [Definition(BuffNames.SeedPack.tutorialDisable)]
    public class TutorialDisableBuff : BuffDefinition
    {
        public TutorialDisableBuff(string nsp, string name) : base(nsp, name)
        {
            AddModifier(new BooleanModifier(SeedPackProperties.DISABLED, ModifyOperator.Set, true));
            AddModifier(new StringModifier(SeedPackProperties.DISABLE_MESSAGE, ModifyOperator.Set, STRING_DISABLE_MESSAGE));
        }
        [TranslateMsg("教程蓝图无法使用提示")]
        public const string STRING_DISABLE_MESSAGE = "暂时无法使用";
    }
}
