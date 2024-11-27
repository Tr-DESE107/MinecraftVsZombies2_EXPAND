using MVZ2.Vanilla;
using MVZ2.Vanilla.SeedPacks;
using PVZEngine.Buffs;
using PVZEngine.Level;
using PVZEngine.Modifiers;

namespace MVZ2.GameContent.Buffs.SeedPacks
{
    [Definition(VanillaBuffNames.SeedPack.easyBlueprint)]
    public class EasyBlueprintBuff : BuffDefinition
    {
        public EasyBlueprintBuff(string nsp, string name) : base(nsp, name)
        {
            AddModifier(new FloatModifier(EngineSeedProps.RECHARGE_SPEED, NumberOperator.Multiply, 1.25f));
        }
    }
}
