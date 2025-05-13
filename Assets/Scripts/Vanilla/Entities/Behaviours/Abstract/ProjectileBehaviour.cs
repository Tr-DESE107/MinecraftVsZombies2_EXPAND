using MVZ2.Vanilla.Callbacks;
using PVZEngine.Callbacks;
using PVZEngine.Damages;
using PVZEngine.Entities;

namespace MVZ2.Vanilla.Entities
{
    public abstract class ProjectileBehaviour : EntityBehaviourDefinition
    {
        protected ProjectileBehaviour(string nsp, string name) : base(nsp, name)
        {
            AddTrigger(VanillaLevelCallbacks.PRE_PROJECTILE_HIT, PreHitEntityCallback);
            AddTrigger(VanillaLevelCallbacks.POST_PROJECTILE_HIT, PostHitEntityCallback);
        }
        private void PreHitEntityCallback(VanillaLevelCallbacks.PreProjectileHitParams param, CallbackResult result)
        {
            var hit = param.hit;
            var projectile = hit.Projectile;
            if (!projectile.Definition.HasBehaviour(this))
                return;
            PreHitEntity(hit, param.damage, result);
        }
        private void PostHitEntityCallback(VanillaLevelCallbacks.PostProjectileHitParams param, CallbackResult result)
        {
            var hit = param.hit;
            var projectile = hit.Projectile;
            if (!projectile.Definition.HasBehaviour(this))
                return;
            PostHitEntity(hit, param.damage);
        }
        protected virtual void PreHitEntity(ProjectileHitInput hit, DamageInput damage, CallbackResult result)
        {
        }
        protected virtual void PostHitEntity(ProjectileHitOutput hitResult, DamageOutput damage)
        {
        }
    }
}