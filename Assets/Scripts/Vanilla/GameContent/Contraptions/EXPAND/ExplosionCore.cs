#nullable enable

using MVZ2.GameContent.Damages;
using MVZ2.GameContent.Effects;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Callbacks;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Level;
using MVZ2Logic.Level;
using PVZEngine.Callbacks;
using PVZEngine.Damages;
using PVZEngine.Entities;
using PVZEngine.Level;
using UnityEngine;
using MVZ2.Vanilla.Callbacks;
using PVZEngine.Callbacks;

namespace MVZ2.GameContent.Contraptions
{
    [EntityBehaviourDefinition(VanillaContraptionNames.ExplosionCore)]
    public class ExplosionCore : ContraptionBehaviour
    {
        public ExplosionCore(string nsp, string name) : base(nsp, name) 
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


            if (!entity.IsEntityOf(VanillaContraptionID.ExplosionCore))
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
            // 根据血量更新受损动画  
            contraption.SetModelDamagePercent();
        }

        protected override void OnTrigger(Entity entity)
        {
            base.OnTrigger(entity);
            entity.PlaySound(VanillaSoundID.gunReload);
            entity.PlaySound(VanillaSoundID.fuse);
            Explode(entity, 160, 600);
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
        // 不能被大招强化  
        public override bool CanEvoke(Entity entity)
        {
            return false;
        }
    }
}