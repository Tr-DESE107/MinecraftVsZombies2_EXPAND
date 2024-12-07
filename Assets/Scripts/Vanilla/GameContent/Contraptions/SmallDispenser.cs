using MVZ2.GameContent.Projectiles;
using MVZ2.Vanilla;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Entities;
using MVZ2Logic.Level;
using PVZEngine.Entities;
using UnityEngine;

namespace MVZ2.GameContent.Contraptions
{
    [Definition(VanillaContraptionNames.smallDispenser)]
    public class SmallDispenser : DispenserFamily
    {
        public SmallDispenser(string nsp, string name) : base(nsp, name)
        {
        }

        public override void Init(Entity entity)
        {
            base.Init(entity);
            InitShootTimer(entity);
        }
        protected override void UpdateAI(Entity entity)
        {
            base.UpdateAI(entity);
            if (!entity.IsEvoked())
            {
                ShootTick(entity);
                return;
            }
        }
        public override Entity Shoot(Entity entity)
        {
            var projectile = base.Shoot(entity);
            projectile.Timeout = Mathf.CeilToInt(entity.GetRange() / entity.GetShotVelocity().magnitude);
            return projectile;
        }

        protected override void OnEvoke(Entity entity)
        {
            base.OnEvoke(entity);
            var velocity = Vector3.right * 3;
            if (entity.IsFacingLeft())
            {
                velocity.x *= -1;
            }
            entity.ShootProjectile(VanillaProjectileID.largeSnowball, velocity);
            entity.PlaySound(VanillaSoundID.odd);
        }
    }
}
