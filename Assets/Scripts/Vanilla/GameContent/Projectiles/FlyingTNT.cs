using MVZ2.GameContent.Contraptions;
using MVZ2.Vanilla;
using MVZ2.Vanilla.Entities;
using PVZEngine.Entities;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

namespace MVZ2.GameContent.Projectiles
{
    [Definition(VanillaProjectileNames.flyingTNT)]
    public class FlyingTNT : VanillaProjectile
    {
        public FlyingTNT(string nsp, string name) : base(nsp, name)
        {
        }
        public override void Init(Entity entity)
        {
            base.Init(entity);
            entity.CollisionMask = 0;
        }
        public override void Update(Entity projectile)
        {
            base.Update(projectile);
            if (projectile.GetRelativeY() <= 0)
            {
                var range = projectile.GetRange();
                var damage = projectile.GetDamage();
                TNT.Explode(projectile, range, damage);
                projectile.Remove();
            }
        }
    }
}
