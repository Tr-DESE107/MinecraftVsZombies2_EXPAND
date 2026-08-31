#nullable enable

using MVZ2.GameContent.Buffs.Contraptions;
using PVZEngine.Definitions;
using MVZ2.GameContent.Damages;
using MVZ2.GameContent.Effects;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Callbacks;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Level;
using MVZ2Logic.Level;
using PVZEngine.Callbacks;
using PVZEngine.Damages;
using UnityEngine;
using PVZEngine.Buffs;
using MVZ2Logic.Entities;
using PVZEngine.Entities;

namespace MVZ2.GameContent.Contraptions
{
    [AutoEntityBehaviourDefinition(VanillaContraptionNames.ExplosionCore_GlowingObsidian)]
    public class ExplosionCore_GlowingObsidian : ContraptionBehaviour
    {
        public ExplosionCore_GlowingObsidian(string nsp, string name) : base(nsp, name) 
        {
            AddTrigger(VanillaLevelCallbacks.PRE_ENTITY_TAKE_DAMAGE, PreEntityTakeDamageCallback);
        }

        public override void Init(Entity entity)
        {
            base.Init(entity);
        }
        private void PreEntityTakeDamageCallback(VanillaLevelCallbacks.PreTakeDamageParams param, CallbackResult result)
        {
            var damageInfo = param.input;
            var entity = damageInfo.Entity;


            if (!entity.IsEntityOf(VanillaContraptionID.ExplosionCore_GlowingObsidian))
                return;

            // 如果伤害包含"爆炸"效果，则减少伤害  
            if (damageInfo.Effects.HasEffect(VanillaDamageEffects.EXPLOSION))
            {
                entity.HealEffects(damageInfo.Amount*2, entity);

                result.SetFinalValue(false);
                damageInfo.Multiply(0f); // 现在level是float类型  
            }


        }
        protected override void UpdateLogic(Entity contraption)
        {
            base.UpdateLogic(contraption);


            // 生命状态动画控制逻辑
            var maxHP = contraption.GetMaxHealth();
            bool netherite = contraption.HasBuff<GlowingObsidianArmorBuff>();
            if (netherite)
            {

                // 如果护甲血量掉到40%，移除Buff
                if (contraption.Health <= maxHP * 0.4f)
                {
                    var hp = contraption.Health;
                    contraption.RemoveBuffs<GlowingObsidianArmorBuff>();
                    netherite = false;
                    contraption.Health = hp;

                    Explode(contraption, 120, 1800);
                    contraption.Level.ShakeScreen(10, 0, 15);
                }
            }

            if (netherite)
            {
                var percent = GetArmoredDamagePercent(contraption, maxHP);
                contraption.SetModelDamagePercent(percent);
            }
            else
            {
                contraption.SetModelDamagePercent();
            }
            contraption.SetModelProperty("Netherite", netherite);
        }
        private float GetArmoredDamagePercent(Entity contraption, float maxHP)
        {
            var percent = contraption.Health / maxHP;
            var armorPercent = (percent - 0.4f) / 0.6f;
            return 1 - armorPercent;
        }
        public override void PostDeath(Entity entity, DeathInfo info)
        {
            base.PostDeath(entity, info);
            var damage = entity.GetDamage()*6;
            var range = entity.GetRange();
            Explode(entity, range, damage);

        }
        protected override void OnTrigger(Entity entity)
        {
            base.OnTrigger(entity);
            entity.PlaySound(VanillaSoundID.gunReload);
            entity.PlaySound(VanillaSoundID.fuse);
            var damage = entity.GetDamage() * 4.5f;
            var range = entity.GetRange();
            Explode(entity, range, damage);
        }
        public static DamageOutput[] Explode(Entity entity, float range, float damage)
        {
            var damageEffects = new DamageEffectList(VanillaDamageEffects.MUTE, VanillaDamageEffects.DAMAGE_BODY_AFTER_ARMOR_BROKEN, VanillaDamageEffects.EXPLOSION);
            var damageOutputs = entity.Level.Explode(entity.Position, range, entity.GetFaction(), damage, damageEffects, entity);
            foreach (var output in damageOutputs)
            {
                var result = output.BodyResult;
                if (result != null && result.Fatal)
                {
                    var target = output.Entity;
                    var distance = (target.Position - entity.Position).magnitude;
                    var speed = 25 * Mathf.Lerp(1f, 0.5f, distance / range);
                    target.Velocity = target.Velocity + Vector3.up * speed;
                }
            }
            Explosion.Spawn(entity, entity.GetCenter(), range);
            entity.PlaySound(VanillaSoundID.explosion);
            entity.Level.ShakeScreen(10, 0, 15);
            entity.Level.Triggers.RunCallbackFiltered(VanillaLevelCallbacks.POST_CONTRAPTION_DETONATE, new EntityCallbackParams(entity), entity.GetDefinitionID());


            return damageOutputs;
        }
        public override bool CanEvoke(Entity entity)
        {
            // 有护甲就不能被大招强化
            if (entity.HasBuff<GlowingObsidianArmorBuff>())
                return false;

            return base.CanEvoke(entity);
        }

        protected override void OnEvoke(Entity contraption)
        {
            base.OnEvoke(contraption);

            // 添加护甲buff
            contraption.AddBuff<GlowingObsidianArmorBuff>();

            // 血量重置
            contraption.Health = contraption.GetMaxHealth();

            // 播放护甲音效
            contraption.Level.PlaySound(VanillaSoundID.armorUp);
        }
    }
}
