#nullable enable

using MVZ2.GameContent.Contraptions;
using MVZ2.GameContent.Enemies;          // VanillaEnemyID.zombieCat  
using MVZ2.GameContent.Entities;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Properties;
using MVZ2Logic.Entities;
using MVZ2Logic.Level;
using PVZEngine.Buffs;
using PVZEngine.Definitions;
using UnityEngine;                       // Vector3 距离计算  
using PVZEngine.Entities;

namespace MVZ2.GameContent.Buffs.Contraptions
{
    [AutoBuffDefinition(VanillaBuffNames.Contraption.HeavyWeaponSelfDestruct)]  // 需在 VanillaBuffNames.Contraption 里加此常量  
    public class HeavyWeaponSelfDestructBuff : BuffDefinition
    {
        public HeavyWeaponSelfDestructBuff(string nsp, string name) : base(nsp, name)
        {
        }
        public override void PostAdd(Buff buff) { base.PostAdd(buff); buff.SetProperty(PROP_TIMEOUT, COUNTDOWN); }
        public override void PostUpdate(Buff buff)
        {
            base.PostUpdate(buff);
            var t = buff.GetProperty<int>(PROP_TIMEOUT) - 1;
            buff.SetProperty(PROP_TIMEOUT, t);

            if (t == 30)
            {
                var e = buff.GetEntity();
                e?.PlaySound(VanillaSoundID.parabotTick);
            }

            if (t <= 0)
            {
                var e = buff.GetEntity();
                if (e != null && !e.IsDead)
                {
                    // 按当前升级等级（射速 + 散射，非狙击器械自动为 0）决定伤害与范围  
                    int upgradeLevel = MegaSnipenser.GetRapidLevel(e) + MegaSnipenser.GetSpreadLevel(e);
                    float range = BASE_RANGE + upgradeLevel * RANGE_PER_LEVEL;
                    float damage = BASE_DAMAGE_MULTIPLIER + upgradeLevel * DAMAGE_MULTIPLIER_PER_LEVEL;

                    HBomb.Explode(e, range, damage);
                    HBomb.ExplodeEffects(e);

                    // 新能力：把爆炸范围内的所有僵尸猫直接移除（无视其闪避）  
                    RemoveZombieCatsInRange(e, range);

                    // 关键：先不移除本 buff，让 Die() 触发的死亡回调能检测到它，从而跳过红石返还  
                    e.Die();
                }
                // Die() 会连带移除实体上的 buff；此处兜底（实体为空/已死时也清理）  
                buff.Remove();
            }
        }

        // 移除爆炸范围内的全部僵尸猫。用 Remove() 而非 Die()，避免触发僵尸猫的闪避/亡语等逻辑，  
        // 直接从场上原子清除，确保它无法靠闪避规避这次自爆。  
        private static void RemoveZombieCatsInRange(Entity source, float range)
        {
            var center = source.GetCenter();
            var sqrRange = range * range;
            foreach (var cat in source.Level.FindEntities(
                e => e.IsEntityOf(VanillaEnemyID.zombieCat)
                     && e.ExistsAndAlive()
                     && (e.GetCenter() - center).sqrMagnitude <= sqrRange))
            {
                cat.Remove();
            }
        }

        public static readonly VanillaBuffPropertyMeta<int> PROP_TIMEOUT = new VanillaBuffPropertyMeta<int>("Timeout");
        public const int COUNTDOWN = 150;                       // 5 秒  
        public const float BASE_RANGE = 240f;                   // 0 级基础爆炸范围  
        public const float RANGE_PER_LEVEL = 60f;               // 每级升级增加的范围  
        public const float BASE_DAMAGE_MULTIPLIER = 1800f;
        public const float DAMAGE_MULTIPLIER_PER_LEVEL = 800f;
    }
}
