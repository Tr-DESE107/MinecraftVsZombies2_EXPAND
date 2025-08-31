using MVZ2.GameContent.Contraptions;
using MVZ2.GameContent.GlobalCallbacks;
using MVZ2.Vanilla.Entities;
using PVZEngine.Buffs;
using PVZEngine.Level;
using PVZEngine.Modifiers;

namespace MVZ2.GameContent.Buffs
{
    [BuffDefinition(VanillaBuffNames.Enemy.iZombieImp)]
    public class IZombieImpBuff : BuffDefinition
    {
        public IZombieImpBuff(string nsp, string name) : base(nsp, name)
        {
            AddModifier(new FloatModifier(VanillaEnemyProps.SPEED, NumberOperator.Multiply, GetSpeedMultiplier()));
            AddModifier(new MaxHealthModifier(NumberOperator.Multiply, 0.25f));
        }
        // 如果要让小鬼能够走过尖刺时稳定被扎两次，那么它的最终速度x应为：
        // x = (s+w)/(60*(1-f)*m)
        // 60是小鬼的移动时间帧数，具体算法为（攻击间隔 * 被扎次数）。
        // 最坏的情况下，小鬼第0帧进入尖刺，0帧就会触发尖刺，然后在30帧后被扎一次，然后在60帧逃脱；
        // 最好的情况下，小鬼第1帧进入尖刺，30帧才会触发尖刺，然后在60帧后被扎一次，然后在61帧逃脱；

        // s为地刺范围宽度，w为小鬼体积宽度，f为小鬼摩擦力，m为怪物移速的乘算倍率。
        // 若s=64，w=16，f=0.15，m=0.4，则x = 3.921569
        // 因为IZ自带所有僵尸速度1.5倍，所以最终还需要除以1.5。
        // 所以最终倍率为 80/(60*0.85*0.4*1.5) = 8000/3060。
        private float GetSpeedMultiplier()
        {
            var spikeWidth = 64;
            var impWidth = 16;
            var spikeInterval = SpikeBlock.ATTACK_COOLDOWN;
            var targetTimes = 2;
            var impFriction = 0.15f;
            var speedFactor = VanillaEnemyExt.WALK_SPEED_FACTOR;
            var izRandomSpeed = IZombieGlobalCallbacks.ZOMBIE_RANDOM_SPEED;
            var baseSpeed = 1;

            var distance = spikeWidth + impWidth;
            var totalTime = spikeInterval * targetTimes;
            var realVelocity = distance / (float)totalTime;

            var frictionMulti = 1 - impFriction;
            var speedValue = realVelocity / frictionMulti / speedFactor / izRandomSpeed;
            return speedValue / baseSpeed;
        }
    }
}
