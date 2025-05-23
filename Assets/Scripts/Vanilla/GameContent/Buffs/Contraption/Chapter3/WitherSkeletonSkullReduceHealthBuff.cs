using PVZEngine.Buffs;
using PVZEngine.Entities;
using PVZEngine.Level;
using PVZEngine.Modifiers;

namespace MVZ2.GameContent.Buffs.Armors
{
    [BuffDefinition(VanillaBuffNames.witherSkeletonSkullReduceHealth)]
    public class WitherSkeletonSkullReduceHealthBuff : BuffDefinition
    {
        public WitherSkeletonSkullReduceHealthBuff(string nsp, string name) : base(nsp, name)
        {
            AddModifier(new MaxHealthModifier(NumberOperator.Multiply, 0.6f));
        }
    }
}
