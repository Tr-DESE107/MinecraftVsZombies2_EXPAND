﻿using MVZ2.GameContent.Buffs.Contraptions;
using MVZ2.GameContent.Damages;
using MVZ2.GameContent.Effects;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Callbacks;
using MVZ2.Vanilla.Contraptions;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Properties;
using MVZ2Logic.Level;
using PVZEngine.Callbacks;
using PVZEngine.Entities;
using PVZEngine.Level;
using Tools;
using UnityEngine;

namespace MVZ2.GameContent.Contraptions
{
    [EntityBehaviourDefinition(VanillaContraptionNames.blackHoleBomb)]
    public class BlackHoleBomb : ContraptionBehaviour
    {
        public BlackHoleBomb(string nsp, string name) : base(nsp, name)
        {
            AddTrigger(LevelCallbacks.POST_ENTITY_DEATH, PostEntityDeathCallback, filter: EntityTypes.PLANT);
        }
        public override void Init(Entity entity)
        {
            base.Init(entity);

            SetExplosionTimer(entity, new FrameTimer(30));
        }
        protected override void UpdateAI(Entity entity)
        {
            base.UpdateAI(entity);
            if (IsIgnited(entity))
            {
                IgnitedUpdate(entity);
            }
        }
        public override bool CanTrigger(Entity entity)
        {
            return base.CanTrigger(entity) && !IsIgnited(entity);
        }
        protected override void OnTrigger(Entity entity)
        {
            base.OnTrigger(entity);
            Ignite(entity);
        }
        protected override void OnEvoke(Entity entity)
        {
            base.OnEvoke(entity);
            entity.SetEvoked(true);
            Ignite(entity);
        }
        private void PostEntityDeathCallback(LevelCallbacks.PostEntityDeathParams param, CallbackResult result)
        {
            var entity = param.entity;
            var info = param.deathInfo;
            if (!entity.Definition.HasBehaviour(this))
                return;
            if (!info.HasEffect(VanillaDamageEffects.SACRIFICE) &&
                !info.HasEffect(VanillaDamageEffects.FIRE) &&
                !info.HasEffect(VanillaDamageEffects.EXPLOSION))
                return;
            var range = entity.GetRange();
            var damage = entity.GetDamage();
            Explode(entity, range, damage);
            entity.Remove();
        }
        public static void Ignite(Entity entity)
        {
            entity.PlaySound(VanillaSoundID.fuse);
            entity.SetBehaviourField(PROP_IGNITED, true);
            entity.AddBuff<TNTIgnitedBuff>();
        }
        public static bool IsIgnited(Entity entity)
        {
            return entity.GetBehaviourField<bool>(PROP_IGNITED);
        }
        public static FrameTimer GetExplosionTimer(Entity entity)
        {
            return entity.GetBehaviourField<FrameTimer>(PROP_EXPLOSION_TIMER);
        }
        public static void SetExplosionTimer(Entity entity, FrameTimer timer)
        {
            entity.SetBehaviourField(PROP_EXPLOSION_TIMER, timer);
        }
        public static Entity Explode(Entity entity, float range, float damage)
        {
            var blackholeParam = entity.GetSpawnParams();
            blackholeParam.SetProperty(VanillaEntityProps.DAMAGE, damage);
            blackholeParam.SetProperty(VanillaEntityProps.RANGE, range);
            var blackhole = entity.Spawn(VanillaEffectID.blackhole, entity.GetCenter(), blackholeParam);

            var explosionParam = entity.GetSpawnParams();
            explosionParam.SetProperty(EngineEntityProps.SIZE, Vector3.one * (range * 2));
            var explosion = entity.Spawn(VanillaEffectID.explosion, entity.GetCenter(), explosionParam);

            entity.PlaySound(VanillaSoundID.explosion);
            entity.PlaySound(VanillaSoundID.gravitation);
            entity.Level.ShakeScreen(10, 0, 15);
            entity.Level.Triggers.RunCallbackFiltered(VanillaLevelCallbacks.POST_CONTRAPTION_DETONATE, new EntityCallbackParams(entity), entity.GetDefinitionID());

            return blackhole;
        }
        public static Entity ExplodeEvoked(Entity entity, float range)
        {
            var fieldParam = entity.GetSpawnParams();
            fieldParam.SetProperty(VanillaEntityProps.RANGE, range);
            var field = entity.Spawn(VanillaEffectID.annihilationField, entity.GetCenter(), fieldParam);

            var explosionParam = entity.GetSpawnParams();
            explosionParam.SetProperty(EngineEntityProps.SIZE, Vector3.one * (range * 2));
            var explosion = entity.Spawn(VanillaEffectID.explosion, entity.GetCenter(), explosionParam);

            entity.PlaySound(VanillaSoundID.explosion);
            entity.PlaySound(VanillaSoundID.gravitation);
            entity.Level.ShakeScreen(10, 0, 15);
            entity.Level.Triggers.RunCallbackFiltered(VanillaLevelCallbacks.POST_CONTRAPTION_DETONATE, new EntityCallbackParams(entity), entity.GetDefinitionID());

            return field;
        }
        private void IgnitedUpdate(Entity entity)
        {
            var timer = GetExplosionTimer(entity);
            timer.Run(entity.GetAttackSpeed());

            if (timer.Frame < 5)
            {
                entity.SetDisplayScale(Vector3.one * Mathf.Lerp(2, 1, timer.Frame / 5f));
            }
            if (timer.Expired)
            {
                var range = entity.GetRange();
                var damage = entity.GetDamage() * DAMAGE_MULTIPLIER;
                if (entity.IsEvoked())
                {
                    ExplodeEvoked(entity, range);
                }
                else
                {
                    Explode(entity, range, damage);
                }
                entity.Remove();
            }
        }
        public const float DAMAGE_MULTIPLIER = 0.01f;
        public static readonly VanillaEntityPropertyMeta<bool> PROP_IGNITED = new VanillaEntityPropertyMeta<bool>("Ignited");
        public static readonly VanillaEntityPropertyMeta<FrameTimer> PROP_EXPLOSION_TIMER = new VanillaEntityPropertyMeta<FrameTimer>("ExplosionTimer");
    }
}
