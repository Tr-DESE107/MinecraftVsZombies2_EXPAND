using MVZ2.Vanilla.Entities;
using PVZEngine.Buffs;
using PVZEngine.Level;
using PVZEngine.Modifiers;

namespace MVZ2.GameContent.Buffs
{
    [BuffDefinition(VanillaBuffNames.iZombieImp)]
    public class IZombieImpBuff : BuffDefinition
    {
        public IZombieImpBuff(string nsp, string name) : base(nsp, name)
        {
            AddModifier(new FloatModifier(VanillaEnemyProps.SPEED, NumberOperator.Multiply, 3200 / 1003f));
            AddModifier(new MaxHealthModifier(NumberOperator.Multiply, 0.25f));
        }
        // 如果要让小鬼能够走过尖刺时稳定被扎两次，那么它的最终速度x应为：
        // x = (s+w)/(59*(1-f)*m)
        // 59是小鬼的移动时间帧数，具体算法为（攻击间隔 * 被扎次数-1）。
        // 最坏的情况下，小鬼第0帧进入尖刺，0帧就会触发尖刺，然后在30帧后被扎一次，然后在59帧逃脱；
        // 最好的情况下，小鬼第1帧进入尖刺，30帧才会触发尖刺，然后在60帧后被扎一次，然后在60帧逃脱；

        // s为地刺范围宽度，w为小鬼体积宽度，f为小鬼摩擦力，m为怪物移速的乘算倍率。
        // 若s=64，w=16，f=0.15，m=0.4，则x = 3.988036
        // 因为IZ自带所有僵尸速度1.25倍，所以最终还需要除以1.25。
        // 所以最终倍率为 3.190429。
    }
}
