using MVZ2.GameContent.Damages;
using MVZ2.GameContent.Effects;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Callbacks;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Level;
using MVZ2Logic.Level;
using PVZEngine;
using PVZEngine.Callbacks;
using PVZEngine.Damages;
using PVZEngine.Entities;
using PVZEngine.Level;
using UnityEngine;

namespace MVZ2.GameContent.Projectiles
{
    [EntityBehaviourDefinition(VanillaEntityBehaviourNames.projectileExplode)]
    public class ProjectileExplodeBehaviour : EntityBehaviourDefinition
    {
        public ProjectileExplodeBehaviour(string nsp, string name) : base(nsp, name)
        {
            AddTrigger(VanillaLevelCallbacks.PRE_PROJECTILE_HIT, PreHitEntityCallback);
        }
        private void PreHitEntityCallback(VanillaLevelCallbacks.PreProjectileHitParams param, CallbackResult result)
        {
            var hit = param.hit;
            var projectile = hit.Projectile;
            if (!projectile.Definition.HasBehaviour(this))
                return;
            param.damage.SetAmount(0);
        }
        public override void PostDeath(Entity entity, DeathInfo damageInfo)
        {
            base.PostDeath(entity, damageInfo);
            if (damageInfo.HasEffect(VanillaDamageEffects.NO_DEATH_TRIGGER))
                return;
            Explode(entity);
        }
        public virtual void Explode(Entity entity)
        {
            ExplodeDamage(entity);
            SpawnExplosionEffect(entity, entity.GetCenter());
            PlayExplosionSound(entity);
        }
        public virtual void ExplodeDamage(Entity entity)
        {
            var range = entity.GetRange();
            var damageEffects = new DamageEffectList(VanillaDamageEffects.EXPLOSION, VanillaDamageEffects.MUTE);
            entity.Explode(entity.Position, range, entity.GetFaction(), entity.GetDamage(), damageEffects);
        }
        public virtual void SpawnExplosionEffect(Entity entity, Vector3 position)
        {
            Explosion.Spawn(entity, entity.GetCenter(), entity.GetRange());
        }
        public virtual void PlayExplosionSound(Entity entity)
        {
            entity.PlaySound(VanillaSoundID.explosion);
        }
    }
}
