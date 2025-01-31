using MVZ2.Vanilla;
using PVZEngine.Buffs;
using PVZEngine.Entities;
using PVZEngine.Modifiers;

namespace MVZ2.GameContent.Buffs.Armors
{
    [Definition(VanillaBuffNames.witherSkeletonSkullReduceHealth)]
    public class WitherSkeletonSkullReduceHealthBuff : BuffDefinition
    {
        public WitherSkeletonSkullReduceHealthBuff(string nsp, string name) : base(nsp, name)
        {
            AddModifier(new FloatModifier(EngineEntityProps.MAX_HEALTH, NumberOperator.Multiply, 0.6f));
        }
    }
}
