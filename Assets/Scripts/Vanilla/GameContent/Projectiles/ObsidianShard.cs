using MVZ2.Vanilla.Entities;
using MVZ2.GameContent.Damages;
using PVZEngine.Damages;
using PVZEngine.Level;
using PVZEngine.Definitions;

namespace MVZ2.GameContent.Projectiles
{
    [AutoEntityBehaviourDefinition(VanillaProjectileNames.ObsidianShard)]
    public class ObsidianShard : ProjectileBehaviour
    {
        public ObsidianShard(string nsp, string name) : base(nsp, name)
        {
        }

        protected override void PostHitEntity(ProjectileHitOutput hitResult, DamageOutput damage)
        {
            base.PostHitEntity(hitResult, damage);

            var target = hitResult.Other;
            var projectile = hitResult.Projectile;

            // ���1�㴩���˺�    
            if (target != null && target.Exists() && !target.IsDead)
            {
                var armorPiercingDamage = new DamageEffectList(VanillaDamageEffects.IGNORE_ARMOR);
                target.TakeDamage(1, armorPiercingDamage, projectile);
            }
        }
    }
}