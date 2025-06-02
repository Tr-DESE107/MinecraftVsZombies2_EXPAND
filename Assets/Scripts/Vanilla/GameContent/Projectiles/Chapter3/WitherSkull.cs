﻿using MVZ2.GameContent.Damages;
using MVZ2.GameContent.Difficulties;
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

namespace MVZ2.GameContent.Projectiles
{
    [EntityBehaviourDefinition(VanillaProjectileNames.witherSkull)]
    public class WitherSkull : ProjectileBehaviour
    {
        public WitherSkull(string nsp, string name) : base(nsp, name)
        {
            AddTrigger(VanillaLevelCallbacks.POST_ENTITY_TAKE_DAMAGE, PostEntityTakeDamageCallback);
        }
        public override void Init(Entity projectile)
        {
            base.Init(projectile);
            projectile.SetModelProperty("Source", projectile.Position);
            projectile.SetModelProperty("Dest", projectile.Position + projectile.Velocity);
        }
        public override void Update(Entity projectile)
        {
            base.Update(projectile);
            projectile.SetModelProperty("Source", projectile.Position);
            projectile.SetModelProperty("Dest", projectile.Position + projectile.Velocity);
        }
        protected override void PreHitEntity(ProjectileHitInput hit, DamageInput damage, CallbackResult result)
        {
            base.PreHitEntity(hit, damage, result);
            damage.SetAmount(0);
        }
        public override void PostDeath(Entity entity, DeathInfo damageInfo)
        {
            base.PostDeath(entity, damageInfo);
            if (damageInfo.Effects.HasEffect(VanillaDamageEffects.REMOVE_ON_DEATH))
                return;
            Explode(entity);
        }
        public static void Explode(Entity entity)
        {
            var range = entity.GetRange();
            entity.PlaySound(VanillaSoundID.explosion);
            var explosion = entity.Level.Spawn(VanillaEffectID.explosion, entity.GetCenter(), entity);
            explosion.SetSize(Vector3.one * (range * 2));
            var damageEffects = new DamageEffectList(VanillaDamageEffects.EXPLOSION, VanillaDamageEffects.MUTE);
            entity.Explode(entity.Position, range, entity.GetFaction(), entity.GetDamage(), damageEffects);
        }
        private void PostEntityTakeDamageCallback(VanillaLevelCallbacks.PostTakeDamageParams param, CallbackResult callbackResult)
        {
            var output = param.output;
            if (output == null)
                return;
            var entity = output.Entity;
            if (entity == null)
                return;
            if (!entity.Level.WitherSkullWithersTarget())
                return;
            if (entity.IsUndead())
                return;
            if (output.BodyResult == null)
                return;
            if (output.BodyResult.Amount <= 0)
                return;
            var source = output.BodyResult.Source;
            if (source != null && source.DefinitionID == GetID())
            {
                entity.InflictWither(WITHER_TIME);
            }
        }
        public const int WITHER_TIME = 900;
    }
}
