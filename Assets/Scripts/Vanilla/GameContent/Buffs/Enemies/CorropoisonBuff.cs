using MVZ2.GameContent.Damages;
using MVZ2.GameContent.Models;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Models;
using MVZ2.Vanilla.Properties;
using MVZ2Logic.Models;
using PVZEngine.Buffs;
using PVZEngine.Damages;
using PVZEngine.Entities;
using PVZEngine.Level;
using UnityEngine;

namespace MVZ2.GameContent.Buffs.Enemies
{
    [BuffDefinition(VanillaBuffNames.Corropoison)]  // 注意这里是你的 Corropoison 的 Buff名字
    public class CorropoisonBuff : BuffDefinition
    {
        public CorropoisonBuff(string nsp, string name) : base(nsp, name)
        {
            // 可以添加回血特效，比如冒绿光之类（可选）
            AddModelInsertion(LogicModelHelper.ANCHOR_CENTER, VanillaModelKeys.CorropoisonParticles, VanillaModelID.CorropoisonParticles);
        }

        public override void PostUpdate(Buff buff)
        {
            base.PostUpdate(buff);

            var entity = buff.GetEntity();
            if (entity != null && !entity.IsDead && entity.Health > 1)
            {
                // 扣除一定量生命
                float damage = buff.GetProperty<float>(PROP_DAMAGE_AMOUNT);
                entity.TakeDamage(damage, new DamageEffectList(VanillaDamageEffects.MUTE, VanillaDamageEffects.NO_DAMAGE_BLINK, VanillaDamageEffects.IGNORE_ARMOR), entity);
            }

            // Buff倒计时逻辑
            var timeout = buff.GetProperty<int>(PROP_TIMEOUT);
            timeout--;
            buff.SetProperty(PROP_TIMEOUT, timeout);

            if (timeout <= 0)
            {
                buff.Remove();
            }
        }

        // PropertyKey注册
        public static readonly VanillaBuffPropertyMeta<float> PROP_DAMAGE_AMOUNT = new VanillaBuffPropertyMeta<float>("DamageAmount", 1f); // 每次回血量
        public static readonly VanillaBuffPropertyMeta<int> PROP_TIMEOUT = new VanillaBuffPropertyMeta<int>("Timeout");       // 持续时间
    }
}
