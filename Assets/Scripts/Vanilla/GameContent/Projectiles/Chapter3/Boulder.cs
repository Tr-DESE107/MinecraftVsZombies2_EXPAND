﻿using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Entities;
using PVZEngine.Damages;
using PVZEngine.Entities;
using PVZEngine.Level;
using UnityEngine;

namespace MVZ2.GameContent.Projectiles
{
    [EntityBehaviourDefinition(VanillaProjectileNames.boulder)]
    public class Boulder : ProjectileBehaviour
    {
        public Boulder(string nsp, string name) : base(nsp, name)
        {
        }
        public override void Update(Entity projectile)
        {
            base.Update(projectile);
            float angleSpeed = -projectile.Velocity.x * 2.5f;
            projectile.RenderRotation += Vector3.forward * angleSpeed;
        }
        protected override void PostHitEntity(ProjectileHitOutput hitResult, DamageOutput damage)
        {
            base.PostHitEntity(hitResult, damage);
            var projectile = hitResult.Projectile;
            var other = hitResult.Other;
            if (other.Type == EntityTypes.ENEMY)
            {
                var vel = other.Velocity;
                vel.x += 5 * Mathf.Sign(projectile.Velocity.x) * other.GetWeakKnockbackMultiplier();
                other.Velocity = vel;
                projectile.PlaySound(VanillaSoundID.bash);
            }

            if (!hitResult.Pierce)
            {
                var rng = projectile.RNG;
                for (int i = 0; i < 3; i++)
                {
                    var randomRotation = new Vector3(rng.Next(360f), rng.Next(360f), rng.Next(360f));
                    var radius = rng.Next(24f);
                    var quaternion = Quaternion.Euler(randomRotation);
                    var pos = projectile.Position + quaternion * Vector3.right * radius;

                    var xspeed = rng.Next(-18f, 18f);
                    var zspeed = rng.Next(-1.5f, 1.5f);
                    var yspeed = rng.Next(10f);

                    var param = new ShootParams()
                    {
                        damage = projectile.GetDamage() * 0.25f,
                        faction = projectile.GetFaction(),
                        position = pos,
                        projectileID = VanillaProjectileID.cobble,
                        velocity = new Vector3(xspeed, yspeed, zspeed),
                    };
                    projectile.ShootProjectile(param);
                }
                projectile.PlaySound(VanillaSoundID.stone);
            }
        }
    }
}
