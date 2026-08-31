#nullable enable // 自动生成

using MVZ2.GameContent.Damages;
using MVZ2.GameContent.Models;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Models;
using MVZ2.Vanilla.Properties;
using MVZ2Logic.Models;
using PVZEngine.Buffs;
using PVZEngine.Damages;
using PVZEngine.Definitions;

namespace MVZ2.GameContent.Buffs.Enemies
{
    [AutoBuffDefinition(VanillaBuffNames.Entity.Corropoison)]  // 注意：这个类是 Corropoison 的 Buff 类
    public class CorropoisonBuff : BuffDefinition
    {
        public CorropoisonBuff(string nsp, string name) : base(nsp, name)
        {
            // 添加腐蚀粒子效果，如冒烟效果之类的（可选）
            AddModelInsertion(LogicModelHelper.ANCHOR_CENTER, VanillaModelKeys.CorropoisonParticles, VanillaModelID.CorropoisonParticles);
        }

        public override void PostUpdate(Buff buff)
        {
            base.PostUpdate(buff);

            var entity = buff.GetEntity();
            if (entity != null && !entity.IsDead && entity.Health > 1)
            {
                // 扣除一定生命值
                float damage = buff.GetProperty<float>(PROP_DAMAGE_AMOUNT);
                entity.TakeDamage(damage, new DamageEffectList(VanillaDamageEffects.MUTE, VanillaDamageEffects.NO_DAMAGE_BLINK, VanillaDamageEffects.IGNORE_ARMOR), entity);
            }

            // Buff过期逻辑
            var timeout = buff.GetProperty<int>(PROP_TIMEOUT);
            timeout--; // 过期时间递减
            buff.SetProperty(PROP_TIMEOUT, timeout);

            if (timeout <= 0)
            {
                buff.Remove(); // 如果过期时间小于等于0，移除Buff
            }
        }

        // PropertyKey注释
        public static readonly VanillaBuffPropertyMeta<float> PROP_DAMAGE_AMOUNT = new VanillaBuffPropertyMeta<float>("DamageAmount", 1f); // 每次腐蚀伤害量
        public static readonly VanillaBuffPropertyMeta<int> PROP_TIMEOUT = new VanillaBuffPropertyMeta<int>("Timeout");       // 持续时间
    }
}
