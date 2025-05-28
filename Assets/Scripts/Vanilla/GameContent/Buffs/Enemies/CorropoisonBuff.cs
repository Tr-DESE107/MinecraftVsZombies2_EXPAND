using MVZ2.GameContent.Damages;
using MVZ2.GameContent.Models;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Models;
using MVZ2.Vanilla.Properties;
using MVZ2Logic.Models;
using PVZEngine.Buffs;
using PVZEngine.Entities;
using PVZEngine.Damages;
using PVZEngine.Level;
using PVZEngine.Modifiers;
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
            if (entity != null && !entity.IsDead)
            {
                // 扣除一定量生命
                float heal = -1f;
                try { heal = buff.GetProperty<float>(Corropoison_HEAL_AMOUNT); } catch { }

                entity.Health = Mathf.Min(entity.Health + heal, entity.GetMaxHealth());
            }

            // Buff倒计时逻辑
            var timeout = buff.GetProperty<int>(Corropoison_TIMEOUT);
            timeout--;
            buff.SetProperty(Corropoison_TIMEOUT, timeout);

            if (timeout <= 0)
            {
                buff.Remove();
            }
        }

        // PropertyKey注册
        public static readonly VanillaBuffPropertyMeta<float> Corropoison_HEAL_AMOUNT = new VanillaBuffPropertyMeta<float>("CorropoisonHealAmount"); // 每次回血量
        public static readonly VanillaBuffPropertyMeta<int> Corropoison_TIMEOUT = new VanillaBuffPropertyMeta<int>("CorropoisonTimeout");       // 持续时间
    }
}
