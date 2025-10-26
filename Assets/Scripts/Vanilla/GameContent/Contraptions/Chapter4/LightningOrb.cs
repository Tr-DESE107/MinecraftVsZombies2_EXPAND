﻿using MVZ2.GameContent.Buffs.Contraptions;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Callbacks;
using MVZ2.Vanilla.Entities;
using MVZ2Logic.Level;
using PVZEngine;
using PVZEngine.Callbacks;
using PVZEngine.Entities;
using PVZEngine.Level;
using MVZ2.GameContent.Buffs;
using MVZ2.GameContent.Bosses;
using MVZ2.GameContent.Buffs.Contraptions;
using MVZ2.GameContent.Damages;
using MVZ2.GameContent.Effects;
using MVZ2.GameContent.Projectiles;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Callbacks;
using MVZ2.Vanilla.Contraptions;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Level;
using MVZ2.Vanilla.Properties;
using MVZ2Logic.Level;
using PVZEngine;
using PVZEngine.Callbacks;
using PVZEngine.Damages;
using PVZEngine.Entities;
using PVZEngine.Level;
using Tools;
using UnityEngine;


namespace MVZ2.GameContent.Contraptions
{
    // 定义一个机关（Contraption）行为：LightningOrb（闪电球）
    [EntityBehaviourDefinition(VanillaContraptionNames.lightningOrb)]
    public class LightningOrb : ContraptionBehaviour
    {
        public LightningOrb(string nsp, string name) : base(nsp, name)
        {
            // 注册一个回调：在投射物击中前触发
            AddTrigger(VanillaLevelCallbacks.PRE_PROJECTILE_HIT, PreProjectileHitCallback);
            AddTrigger(VanillaLevelCallbacks.PRE_ENTITY_TAKE_DAMAGE, PreEntityTakeDamageCallback);
        }

        //public override void Init(Entity entity)
        //{
        //    base.Init(entity);
        //    var buff = entity.AddBuff<ExplosionProtection>();
        //    buff.SetProperty(ExplosionProtection.PROP_Protection_Level, 1f);
        //}

        // 每帧更新逻辑
        protected override void UpdateLogic(Entity contraption)
        {
            base.UpdateLogic(contraption);

            // 更新动画参数
            contraption.SetAnimationFloat("Damaged", 1 - contraption.Health / contraption.GetMaxHealth());
            contraption.SetAnimationBool("Absorbing", contraption.HasBuff<LightningOrbEvokedBuff>());
        }

        // 投射物即将命中时的回调
        private void PreProjectileHitCallback(VanillaLevelCallbacks.PreProjectileHitParams param, CallbackResult result)
        {
            var hit = param.hit;
            var damage = param.damage;

            // 如果伤害目标是护盾，就直接退出
            if (NamespaceID.IsValid(damage.ShieldTarget))
                return;

            var orb = hit.Other;
            // 确认命中的实体确实是 LightningOrb
            if (!orb.Definition.HasBehaviour(this))
                return;

            var projectile = hit.Projectile;

            // 吸收投射物伤害 → 回复自身生命
            orb.HealEffects(100, projectile);

            // 将伤害量累积到 buff 的 TakenDamage
            foreach (var buff in orb.GetBuffs<LightningOrbEvokedBuff>())
            {
                LightningOrbEvokedBuff.AddTakenDamage(buff, damage.Amount);
            }

            // 投射物被销毁
            projectile.Remove();

            // 阻止投射物继续造成伤害
            result.SetFinalValue(false);

            // 播放护盾被击中的音效
            orb.PlaySound(VanillaSoundID.energyShieldHit);
        }

        private void PreEntityTakeDamageCallback(VanillaLevelCallbacks.PreTakeDamageParams param, CallbackResult result)
        {
            var damageInfo = param.input;
            var entity = damageInfo.Entity;


            if (!entity.IsEntityOf(VanillaContraptionID.lightningOrb))
                return;

            // 如果伤害包含"爆炸"效果，则减少伤害  
            if (damageInfo.Effects.HasEffect(VanillaDamageEffects.EXPLOSION))
                {
                    entity.HealEffects(damageInfo.Amount, entity);
                    
                    result.SetFinalValue(false);
                    damageInfo.Multiply(0f); // 现在level是float类型  
                }
            

        }

        // 决定是否可以被唤醒
        public override bool CanEvoke(Entity entity)
        {
            // 如果已经存在 Evoked buff，就不能再次唤醒
            if (entity.HasBuff<LightningOrbEvokedBuff>())
                return false;
            return base.CanEvoke(entity);
        }

        // 当被唤醒时
        protected override void OnEvoke(Entity entity)
        {
            base.OnEvoke(entity);

            // 播放唤醒音效
            entity.PlaySound(VanillaSoundID.lightningAttack);

            // 赋予 Evoked buff（进入吸收状态）
            entity.AddBuff<LightningOrbEvokedBuff>();
        }

        // 固定吸收一次投射物回复的生命值
        public const float HEAL_AMOUNT = 100;
    }
}

