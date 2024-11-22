using MVZ2.Vanilla;
using MVZ2.Vanilla.Entities;
using PVZEngine.Entities;
using UnityEngine;

namespace MVZ2.GameContent.Projectiles
{
    [Definition(VanillaProjectileNames.largeSnowball)]
    public class LargeSnowball : VanillaProjectile
    {
        public LargeSnowball(string nsp, string name) : base(nsp, name)
        {
        }
        public override void Init(Entity entity)
        {
            base.Init(entity);
            entity.SetProperty(PROP_SNOWBALL_SCALE, 1f);
        }
        public override void Update(Entity projectile)
        {
            base.Update(projectile);

            projectile.Velocity += projectile.Velocity.normalized / 3f;
            var scale = projectile.GetProperty<float>(PROP_SNOWBALL_SCALE);
            scale = Mathf.Clamp(scale + SCALE_SPEED, MIN_SCALE, MAX_SCALE);
            projectile.SetProperty(PROP_SNOWBALL_SCALE, scale);

            float angleSpeed = -projectile.Velocity.x * 2.5f;
            projectile.RenderRotation += Vector3.forward * angleSpeed;

            Vector3 scaleVector = new Vector3(scale, scale, 1);
            projectile.Scale = scaleVector;
            projectile.RenderScale = scaleVector;
            projectile.SetShadowScale(scaleVector * 0.5f);
            projectile.SetDamage(Mathf.Max(0, (scale - 1) * 300));
        }
        public const string PROP_SNOWBALL_SCALE = "SnowballScale";
        public const float MIN_SCALE = 1;
        public const float SCALE_SPEED = 0.1f;
        public const float MAX_SCALE = 10;
    }
}
