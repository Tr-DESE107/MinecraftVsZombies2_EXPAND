using MVZ2.GameContent.Models;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Models;
using MVZ2.Vanilla.Properties;
using MVZ2Logic.Models;
using PVZEngine.Buffs;
using PVZEngine.Entities;
using PVZEngine.Level;
using UnityEngine;
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
    [BuffDefinition(VanillaBuffNames.Regeneration)]  // 注意这里是你的 Regeneration 的 Buff名字
    public class RegenerationBuff : BuffDefinition
    {
        public RegenerationBuff(string nsp, string name) : base(nsp, name)
        {
            // 可以添加回血特效，比如冒绿光之类（可选）
            AddModelInsertion(LogicModelHelper.ANCHOR_CENTER, VanillaModelKeys.RegenerationParticles, VanillaModelID.RegenerationParticles);
        }

        public override void PostUpdate(Buff buff)
        {
            base.PostUpdate(buff);

            var entity = buff.GetEntity();
            if (entity != null && !entity.IsDead)
            {
                // 恢复一定量生命
                float heal = buff.GetProperty<float>(PROP_HEAL_AMOUNT);
                if (heal >= 0f)
                {
                    entity.Heal(heal, entity);
                }
                else if (heal < 0f)
                {
                    entity.TakeDamage(heal * -1, new DamageEffectList(VanillaDamageEffects.MUTE, VanillaDamageEffects.NO_DAMAGE_BLINK, VanillaDamageEffects.IGNORE_ARMOR), entity);
                }
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
        public static readonly VanillaBuffPropertyMeta<float> PROP_HEAL_AMOUNT = new VanillaBuffPropertyMeta<float>("HealAmount", 1f); // 每次回血量
        public static readonly VanillaBuffPropertyMeta<int> PROP_TIMEOUT = new VanillaBuffPropertyMeta<int>("Timeout");       // 持续时间
    }
}
