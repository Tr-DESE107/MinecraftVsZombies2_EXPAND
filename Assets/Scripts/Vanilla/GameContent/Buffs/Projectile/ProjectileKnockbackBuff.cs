using MVZ2.GameContent.Models;
using MVZ2.Vanilla.Callbacks;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Models;
using MVZ2Logic.Models;
using PVZEngine.Buffs;
using PVZEngine.Damages;
using PVZEngine.Entities;
using PVZEngine.Level;
using UnityEngine;

namespace MVZ2.GameContent.Buffs.Projectiles
{
    [BuffDefinition(VanillaBuffNames.projectileKnockback)]
    public class ProjectileKnockbackBuff : BuffDefinition
    {
        public ProjectileKnockbackBuff(string nsp, string name) : base(nsp, name)
        {
            AddModelInsertion(LogicModelHelper.ANCHOR_CENTER, VanillaModelKeys.knockbackWave, VanillaModelID.knockbackWave);
            AddTrigger(VanillaLevelCallbacks.POST_PROJECTILE_HIT, PostProjectileHitCallback);
        }
        private void PostProjectileHitCallback(ProjectileHitOutput hitOutput, DamageOutput damage)
        {
            var projectile = hitOutput.Projectile;
            if (projectile == null)
                return;
            if (!projectile.HasBuff<ProjectileKnockbackBuff>())
                return;
            var other = hitOutput.Other;
            if (other.Type == EntityTypes.ENEMY)
            {
                var sign = Mathf.Sign(projectile.Velocity.x);
                other.Velocity += new Vector3(1 * sign, 1);
            }
        }
    }
}
