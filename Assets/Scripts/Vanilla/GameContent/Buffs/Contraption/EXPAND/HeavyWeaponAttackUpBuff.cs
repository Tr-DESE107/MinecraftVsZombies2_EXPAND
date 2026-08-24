#nullable enable

using MVZ2.Vanilla.Entities;      // VanillaEntityProps.DAMAGE  
using MVZ2.Vanilla.Properties;    // VanillaBuffPropertyMeta  
using PVZEngine.Buffs;
using PVZEngine.Definitions;
using PVZEngine.Modifiers;
using UnityEngine;                // Mathf  

namespace MVZ2.GameContent.Buffs.Contraptions
{
    // 攻击力升级 buff：挂在矿车上的骑乘器械身上，按等级用乘法叠加伤害。  
    // 每级 +20%（DAMAGE_PER_LEVEL），满级 3 级（MAX_LEVEL），两个常量均可改。  
    // 例：1级=×1.2，2级=×1.4，3级=×1.6。  
    [AutoBuffDefinition(VanillaBuffNames.Contraption.HeavyWeaponAttackUp)]
    public class HeavyWeaponAttackUpBuff : BuffDefinition
    {
        public HeavyWeaponAttackUpBuff(string nsp, string name) : base(nsp, name)
        {
            // 伤害乘以 PROP_MULTIPLIER（默认 1，即无影响，直到升级后被写入）  
            AddModifier(new FloatModifier(VanillaEntityProps.DAMAGE, NumberOperator.Multiply, PROP_MULTIPLIER));
        }

        // buff 刚挂上时初始化为 0 级、倍率 1（不改变伤害）  
        public override void PostAdd(Buff buff)
        {
            base.PostAdd(buff);
            SetLevel(buff, 0);
            buff.SetProperty(PROP_MULTIPLIER, 1f);
        }

        // 升一级（封顶到 MAX_LEVEL），并按新等级刷新伤害倍率  
        public static void Upgrade(Buff buff)
        {
            int level = Mathf.Min(GetLevel(buff) + 1, MAX_LEVEL);
            SetLevel(buff, level);
            buff.SetProperty(PROP_MULTIPLIER, 1f + level * DAMAGE_PER_LEVEL);
        }

        public static int GetLevel(Buff buff) => buff.GetProperty<int>(PROP_LEVEL);
        public static void SetLevel(Buff buff, int value) => buff.SetProperty(PROP_LEVEL, value);

        // ===== 可调参数 =====  
        public const float DAMAGE_PER_LEVEL = 0.2f;   // 每级增量 20%（可改）  
        public const int MAX_LEVEL = 3;               // 满级上限 3 级（可改）  

        public static readonly VanillaBuffPropertyMeta<int> PROP_LEVEL =
            new VanillaBuffPropertyMeta<int>("attack_up_level");
        // 默认 1f：未升级时倍率为 1，不影响伤害  
        public static readonly VanillaBuffPropertyMeta<float> PROP_MULTIPLIER =
            new VanillaBuffPropertyMeta<float>("attack_up_multiplier", 1f);
    }
}
