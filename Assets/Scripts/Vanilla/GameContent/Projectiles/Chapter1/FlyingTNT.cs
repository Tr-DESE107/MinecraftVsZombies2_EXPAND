﻿using MVZ2.GameContent.Contraptions;
using MVZ2.Vanilla.Entities;
using PVZEngine.Entities;
using PVZEngine.Level;

namespace MVZ2.GameContent.Projectiles
{
    [EntityBehaviourDefinition(VanillaProjectileNames.flyingTNT)]
    public class FlyingTNT : ProjectileBehaviour
    {
        public FlyingTNT(string nsp, string name) : base(nsp, name)
        {
        }
        public override void Init(Entity entity)
        {
            base.Init(entity);
            entity.CollisionMaskHostile = 0;
            entity.CollisionMaskFriendly = 0;
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
