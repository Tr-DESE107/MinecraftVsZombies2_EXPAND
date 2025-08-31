using MVZ2.Vanilla.Entities;
using PVZEngine.Buffs;
using PVZEngine.Level;
using PVZEngine.Modifiers;

namespace MVZ2.GameContent.Buffs
{
    [BuffDefinition(VanillaBuffNames.Enemy.iZombieSkeletonWarrior)]
    public class IZombieSkeletonWarriorBuff : BuffDefinition
    {
        public IZombieSkeletonWarriorBuff(string nsp, string name) : base(nsp, name)
        {
            AddModifier(new FloatModifier(VanillaEnemyProps.SPEED, NumberOperator.Multiply, 2f));
        }
    }
}
