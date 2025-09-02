using MVZ2.GameContent.Damages;
using MVZ2.GameContent.Models;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Models;
using MVZ2.Vanilla.Properties;
using MVZ2Logic.Models;
using PVZEngine.Buffs;
using PVZEngine.Damages;
using PVZEngine.Level;
using MVZ2.Vanilla.Audios;
using MVZ2Logic.Level;
using PVZEngine.Entities;
using UnityEngine;
using MVZ2.GameContent.Difficulties;
using MVZ2.GameContent.Effects;
using MVZ2.Vanilla.Level;
using MVZ2.Vanilla.Callbacks;
using MVZ2.Vanilla.Contraptions;
using PVZEngine.Callbacks;

namespace MVZ2.GameContent.Buffs.Enemies
{
    [BuffDefinition(VanillaBuffNames.withered)]
    public class WitheredBuff : BuffDefinition
    {
        public WitheredBuff(string nsp, string name) : base(nsp, name)
        {
            // 在模型中心插入凋零粒子特效
            AddModelInsertion(LogicModelHelper.ANCHOR_CENTER, VanillaModelKeys.witherParticles, VanillaModelID.witherParticles);

            // 监听伤害回调（用于传播）
            AddTrigger(VanillaLevelCallbacks.POST_ENTITY_TAKE_DAMAGE, PostEntityTakeDamageCallback);
        }

        /// <summary>
        /// 每帧更新：持续掉血、到时清除
        /// </summary>
        public override void PostUpdate(Buff buff)
        {
            base.PostUpdate(buff);

            var entity = buff.GetEntity();
            if (entity != null)
            {
                // 持续伤害（无视护甲 & 静音）
                entity.TakeDamage(WITHER_DAMAGE, new DamageEffectList(VanillaDamageEffects.IGNORE_ARMOR, VanillaDamageEffects.MUTE, VanillaDamageEffects.WITHER), entity);

                // 当生命值小于等于 1 → 爆炸
                if (entity.Health <= 1)
                {
                    Explode(entity);
                    entity.Remove();
                }
            }

            // 递减 buff 存活时间
            var timeout = buff.GetProperty<int>(PROP_TIMEOUT);
            timeout--;
            buff.SetProperty(PROP_TIMEOUT, timeout);
            if (timeout <= 0)
            {
                buff.Remove();
            }
        }

        // ==== 属性 ====

        public static readonly VanillaBuffPropertyMeta<int> PROP_TIMEOUT = new VanillaBuffPropertyMeta<int>("Timeout");

        public const float WITHER_DAMAGE = 1 / 3f;
        public const int WITHER_TIME = 900;

        // ==== 自爆 ====

        public static void Explode(Entity entity)
        {
            var range = entity.GetRange();

            // 播放音效 & 生成爆炸特效
            entity.PlaySound(VanillaSoundID.explosion);
            var explosion = entity.Level.Spawn(VanillaEffectID.explosion, entity.GetCenter(), entity);
            explosion.SetSize(Vector3.one * (range * 2));

            // 关键点：爆炸伤害附带 WITHER 标记
            var damageEffects = new DamageEffectList(
                VanillaDamageEffects.EXPLOSION,
                VanillaDamageEffects.MUTE,
                VanillaDamageEffects.WITHER // ✅ 标记这是 Withered 爆炸
            );

            // 爆炸伤害
            entity.Explode(entity.Position, 120, VanillaFactions.NEUTRAL, 30, damageEffects);
        }

        // ==== 传播逻辑 ====

        private void PostEntityTakeDamageCallback(VanillaLevelCallbacks.PostTakeDamageParams param, CallbackResult callbackResult)
        {
            var output = param.output;
            if (output == null)
                return;

            var entity = output.Entity;
            if (entity == null)
                return;

            // 游戏设置里是否允许凋零效果
            if (!entity.Level.WitherSkullWithersTarget())
                return;

            

            // 是否有伤害结果
            if (output.BodyResult == null)
                return;
            if (output.BodyResult.Amount <= 0)
                return;

            // 检查是否来自带有 WITHER 标记的爆炸
            //if (result.BodyResult.Effects.HasEffect(VanillaDamageEffects.LIGHTNING))
            if (output.BodyResult.Effects.HasEffect(VanillaDamageEffects.WITHER))
            {
                // 给目标附加 Withered 效果  
                entity.InflictWither(WITHER_TIME);
            }
        }
    }
}
