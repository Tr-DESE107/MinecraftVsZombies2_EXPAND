using MVZ2.GameContent.Damages;
using MVZ2.GameContent.Effects;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Level;
using MVZ2Logic.Level;
using PVZEngine.Callbacks;
using PVZEngine.Damages;
using PVZEngine.Entities;
using PVZEngine.Level;

namespace MVZ2.GameContent.Projectiles
{
    [EntityBehaviourDefinition(VanillaProjectileNames.beaconMeteor)]
    public class BeaconMeteor : ProjectileBehaviour
    {
        public BeaconMeteor(string nsp, string name) : base(nsp, name)
        {
        }
        protected override void PreHitEntity(ProjectileHitInput hit, DamageInput damage, CallbackResult result)
        {
            base.PreHitEntity(hit, damage, result);
            damage.SetAmount(0);
        }
        public override void PostDeath(Entity entity, DeathInfo damageInfo)
        {
            base.PostDeath(entity, damageInfo);
            if (damageInfo.HasEffect(VanillaDamageEffects.NO_DEATH_TRIGGER))
                return;
            Explode(entity);
        }
        public static void Explode(Entity entity)
        {
            var range = entity.GetRange();
            Explosion.Spawn(entity, entity.GetCenter(), range);

            var damageEffects = new DamageEffectList(VanillaDamageEffects.EXPLOSION, VanillaDamageEffects.MUTE);
            entity.Explode(entity.Position, range, entity.GetFaction(), entity.GetDamage(), damageEffects);

            entity.Level.ShakeScreen(10, 0, 15);
            entity.PlaySound(VanillaSoundID.explosion);
        }
    }
}
