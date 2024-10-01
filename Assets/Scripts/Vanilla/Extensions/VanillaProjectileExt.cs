using MVZ2.Extensions;
using PVZEngine.Level;
using UnityEngine;

namespace MVZ2.Vanilla
{
    public static class VanillaProjectileExt
    {
        public static Entity ShootProjectile(this Entity entity)
        {
            var game = entity.Level;
            entity.PlaySound(entity.GetShootSound());

            Vector3 offset = entity.GetShotOffset();
            Vector3 velocity = entity.GetShotVelocity();
            if (entity.IsFacingLeft())
            {
                offset.x *= -1;
                velocity.x *= -1;
            }
            velocity = entity.ModifyProjectileVelocity(velocity);

            var projectile = game.Spawn(entity.GetProjectileID(), entity.Pos + offset, entity);
            projectile.SetDamage(entity.GetDamage());
            projectile.SetFaction(entity.GetFaction());
            projectile.Velocity = velocity;
            return projectile;
        }
    }
}
