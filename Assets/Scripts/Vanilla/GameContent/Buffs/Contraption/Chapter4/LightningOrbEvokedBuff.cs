﻿using MVZ2.GameContent.Contraptions;
using MVZ2.GameContent.Damages;
using MVZ2.GameContent.Detections;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Callbacks;
using MVZ2.Vanilla.Detections;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Level;
using MVZ2.Vanilla.Properties;
using PVZEngine.Base;
using PVZEngine.Buffs;
using PVZEngine.Callbacks;
using PVZEngine.Damages;
using PVZEngine.Entities;
using PVZEngine.Level;
using Tools;
using UnityEngine;

namespace MVZ2.GameContent.Buffs.Contraptions
{
    // 定义 Buff：LightningOrbEvoked（闪电球进入“唤醒/吸收”状态）
    [BuffDefinition(VanillaBuffNames.lightningOrbEvoked)]
    public class LightningOrbEvokedBuff : BuffDefinition
    {
        public LightningOrbEvokedBuff(string nsp, string name) : base(nsp, name)
        {
            // 注册触发器：在实体受到伤害前调用
            AddTrigger(VanillaLevelCallbacks.PRE_PROJECTILE_HIT, PreProjectileHitCallback);
            thunderDetector = new LawnDetector();
        }

        // Buff 添加时调用
        public override void PostAdd(Buff buff)
        {
            base.PostAdd(buff);
            // 给 Buff 设置一个计时器，150 帧后触发释放
            buff.SetProperty(PROP_TIMER, new FrameTimer(MAX_TIMEOUT));
        }

        // 每帧更新 buff
        public override void PostUpdate(Buff buff)
        {
            base.PostUpdate(buff);
            var timer = buff.GetProperty<FrameTimer>(PROP_TIMER);
            var entity = buff.GetEntity();

            // 如果计时结束 → 触发雷击释放
            if (timer == null || timer.Expired)
            {
                var damage = GetTakenDamage(buff);
                damage = Mathf.Min(damage, MAX_DAMAGE);

                if (damage > 0)
                {
                    var level = buff.Level;
                    // 全局雷电特效
                    level.Thunder();

                    if (entity != null)
                    {
                        thunderBuffer.Clear();
                        // 探测附近目标
                        thunderDetector.DetectMultiple(entity, thunderBuffer);

                        // 给探测到的敌人造成伤害
                        for (int i = 0; i < thunderBuffer.Count; i++)
                        {
                            var collider = thunderBuffer[i];
                            collider.TakeDamage(
                                damage,
                                new DamageEffectList(
                                    VanillaDamageEffects.DAMAGE_BODY_AFTER_ARMOR_BROKEN,
                                    VanillaDamageEffects.LIGHTNING
                                ),
                                entity
                            );
                        }

                        // 中心特效 + 爆炸弧线
                        TNT.ExplodeArcs(entity, entity.GetCenter());

                        // 多个雷电相关音效
                        entity.PlaySound(VanillaSoundID.thunder);
                        entity.PlaySound(VanillaSoundID.tridentThunder);
                        entity.PlaySound(VanillaSoundID.teslaAttack);
                    }
                }

                // Buff 移除（一次性技能）
                buff.Remove();
            }
            else
            {
                // Buff 仍在计时中
                timer.Run();

                // 每 15 帧发出护盾音效，音调逐渐升高
                if (entity != null && entity.IsTimeInterval(15))
                {
                    var pitch = 1 + (1 - timer.Frame) / (float)MAX_TIMEOUT;
                    entity.PlaySound(VanillaSoundID.energyShield, pitch);
                }
            }
        }

        // 在实体受到伤害前触发
        private void PreProjectileHitCallback(VanillaLevelCallbacks.PreProjectileHitParams param, CallbackResult result)
        {
            var hit = param.hit;
            var projectile = hit.Projectile;
            var damage = param.damage;
            var entity = hit.Other; // 命中的对象

            foreach (var buff in entity.GetBuffs<LightningOrbEvokedBuff>())
            {
                // ✅ 只拦截投射物（因为这是 PRE_PROJECTILE_HIT 回调，不会触发近战）
                entity.HealEffects(damage.Amount, entity);
                AddTakenDamage(buff, damage.Amount);
                result.SetFinalValue(false); // 阻止原始命中
            }
            projectile.Remove();
        }


        // 辅助方法：管理 Buff 的累积伤害数值
        public static float GetTakenDamage(Buff buff) => buff.GetProperty<float>(PROP_TAKEN_DAMAGE);
        public static void SetTakenDamage(Buff buff, float value) => buff.SetProperty(PROP_TAKEN_DAMAGE, value);
        public static void AddTakenDamage(Buff buff, float value) => SetTakenDamage(buff, GetTakenDamage(buff) + value);

        // 常量定义
        public const int MAX_TIMEOUT = 150; // buff 存续时间
        public const float MAX_DAMAGE = 900; // 雷击伤害上限

        // Buff 内部属性键
        public static readonly VanillaBuffPropertyMeta<FrameTimer> PROP_TIMER = new VanillaBuffPropertyMeta<FrameTimer>("timer");
        public static readonly VanillaBuffPropertyMeta<float> PROP_TAKEN_DAMAGE = new VanillaBuffPropertyMeta<float>("takenDamage");

        // 探测器与缓存
        private Detector thunderDetector;
        private ArrayBuffer<IEntityCollider> thunderBuffer = new ArrayBuffer<IEntityCollider>(1024);
    }
}
