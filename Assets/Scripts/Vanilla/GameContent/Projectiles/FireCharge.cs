using MVZ2.GameContent.Damages;
using MVZ2.GameContent.Effects;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Level;
using MVZ2Logic.Level;
using PVZEngine;
using PVZEngine.Damages;
using PVZEngine.Entities;
using PVZEngine.Level;
using UnityEngine;

namespace MVZ2.GameContent.Projectiles
{
    [EntityBehaviourDefinition(VanillaProjectileNames.fireCharge)]
    public class FireCharge : ProjectileBehaviour
    {
        public FireCharge(string nsp, string name) : base(nsp, name)
        {
        }
        protected override void PreHitEntity(ProjectileHitInput hit, DamageInput damage)
        {
            base.PreHitEntity(hit, damage);
            var entity = hit.Projectile;
            Explode(entity);
            damage.Cancel();
        }
        public override void PostContactGround(Entity entity, Vector3 velocity)
        {
            base.PostContactGround(entity, velocity);
            Explode(entity);
            entity.Remove();
        }
        public static void Explode(Entity entity)
        {
            var range = entity.GetRange();
            entity.PlaySound(VanillaSoundID.explosion);
            var explosion = entity.Level.Spawn(VanillaEffectID.explosion, entity.GetCenter(), entity);
            explosion.SetSize(Vector3.one * (range * 2));
            var damageEffects = new DamageEffectList(VanillaDamageEffects.EXPLOSION, VanillaDamageEffects.MUTE);
            entity.Level.Explode(entity.Position, range, entity.GetFaction(), entity.GetDamage(), damageEffects, entity);
        }
        public static NamespaceID ID => VanillaProjectileID.fireCharge;
    }
}
