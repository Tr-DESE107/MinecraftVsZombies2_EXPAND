﻿using MVZ2.Vanilla.Entities;
using PVZEngine.Damages;
using PVZEngine.Entities;
using PVZEngine.Level;

namespace MVZ2.GameContent.Projectiles
{
    [EntityBehaviourDefinition(VanillaProjectileNames.largeArrow)]
    public class LargeArrow : ProjectileBehaviour
    {
        public LargeArrow(string nsp, string name) : base(nsp, name)
        {
        }
        public override void Update(Entity projectile)
        {
            base.Update(projectile);
            projectile.Velocity += projectile.Velocity.normalized * 0.3f;

            var rotation = projectile.RenderRotation;
            rotation.x += projectile.Velocity.magnitude * 30;
            rotation.x %= 360;
            projectile.RenderRotation = rotation;
        }
        protected override void PostHitEntity(ProjectileHitOutput hitResult, DamageOutput damage)
        {
            base.PostHitEntity(hitResult, damage);
            if (damage == null)
                return;
            var projecitle = hitResult.Projectile;
            bool fatal = true;
            float spentDamage = 0;
            CheckDamageResult(damage.ShieldResult);
            CheckDamageResult(damage.ArmorResult);
            CheckDamageResult(damage.BodyResult);

            if (!fatal)
            {
                projecitle.Remove();
            }

            var dmg = projecitle.GetDamage();
            dmg -= spentDamage;
            projecitle.SetDamage(dmg);

            if (dmg <= 0)
            {
                projecitle.Remove();
            }

            void CheckDamageResult(DamageResult result)
            {
                if (result != null)
                {
                    if (result.Fatal)
                    {
                        spentDamage += result.SpendAmount;
                    }
                    else
                    {
                        fatal = false;
                    }
                }
            }
        }
    }
}
