﻿using MVZ2.GameContent.Damages;
using MVZ2.GameContent.Effects;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Level;
using PVZEngine.Damages;
using PVZEngine.Entities;
using PVZEngine.Level;

namespace MVZ2.GameContent.Projectiles
{
    [EntityBehaviourDefinition(VanillaProjectileNames.fireball)]
    public class Fireball : ProjectileBehaviour
    {
        public Fireball(string nsp, string name) : base(nsp, name)
        {
        }
        protected override void PostHitEntity(ProjectileHitOutput hitResult, DamageOutput damageOutput)
        {
            base.PostHitEntity(hitResult, damageOutput);
            if (damageOutput == null)
                return;
            var entity = hitResult.Projectile;
            var other = hitResult.Other;

            bool blocksFire = damageOutput.WillDamageBlockFire();

            if (!blocksFire)
            {
                var damageEffects = new DamageEffectList(VanillaDamageEffects.FIRE, VanillaDamageEffects.MUTE);
                entity.SplashDamage(hitResult.Collider, entity.Position, 40, entity.GetFaction(), entity.GetDamage() / 4f, damageEffects);
                entity.Spawn(VanillaEffectID.fireburn, entity.Position);
            }
        }
    }
}
