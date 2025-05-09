using MVZ2.GameContent.Damages;
using MVZ2.GameContent.Effects;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Level;
using MVZ2.Vanilla.Properties;
using MVZ2.Vanilla.Shells;
using MVZ2Logic.Level;
using PVZEngine;
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
                entity.Level.Explode(entity.Position, 40, entity.GetFaction(), entity.GetDamage() / 3f, damageEffects, entity);
                entity.Level.Spawn(VanillaEffectID.fireburn, entity.Position, entity);
            }
        }
    }
}
