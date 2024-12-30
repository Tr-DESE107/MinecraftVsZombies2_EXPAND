using MVZ2.Vanilla;
using PVZEngine.Buffs;
using PVZEngine.Level;
using PVZEngine.Modifiers;

namespace MVZ2.GameContent.Buffs.SeedPacks
{
    [Definition(VanillaBuffNames.SeedPack.theCreaturesHeartReduceCost)]
    public class TheCreaturesHeartReduceCostBuff : BuffDefinition
    {
        public TheCreaturesHeartReduceCostBuff(string nsp, string name) : base(nsp, name)
        {
            AddModifier(new FloatModifier(EngineSeedProps.COST, NumberOperator.Add, PROP_ADDITION));
        }
        public const string PROP_ADDITION = "Reduction";
    }
}
