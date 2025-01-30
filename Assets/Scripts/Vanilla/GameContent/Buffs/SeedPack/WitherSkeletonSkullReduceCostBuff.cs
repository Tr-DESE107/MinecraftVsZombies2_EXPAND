using MVZ2.Vanilla;
using PVZEngine.Buffs;
using PVZEngine.Level;
using PVZEngine.Modifiers;

namespace MVZ2.GameContent.Buffs.SeedPacks
{
    [Definition(VanillaBuffNames.SeedPack.witherSkeletonSkullReduceCost)]
    public class WitherSkeletonSkullReduceCostBuff : BuffDefinition
    {
        public WitherSkeletonSkullReduceCostBuff(string nsp, string name) : base(nsp, name)
        {
            AddModifier(new FloatModifier(EngineSeedProps.COST, NumberOperator.Multiply, 0.8f));
        }
    }
}
