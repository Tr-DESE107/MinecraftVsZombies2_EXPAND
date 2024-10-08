using MVZ2.Vanilla;
using PVZEngine.Definitions;
using PVZEngine.Level;
using PVZEngine.Modifiers;

namespace MVZ2.GameContent.Buffs.SeedPack
{
    [Definition(VanillaBuffNames.SeedPack.easyBlueprint)]
    public class EasyBlueprintBuff : BuffDefinition
    {
        public EasyBlueprintBuff(string nsp, string name) : base(nsp, name)
        {
            AddModifier(new FloatModifier(EngineSeedProps.RECHARGE_SPEED, NumberOperator.Multiply, 1.25f));
            AddModifier(new IntModifier(BuiltinSeedProps.TRIGGER_COST, NumberOperator.Multiply, 0));
        }
    }
}
