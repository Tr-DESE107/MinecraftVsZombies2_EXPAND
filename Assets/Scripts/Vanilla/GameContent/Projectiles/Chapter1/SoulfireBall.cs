using MVZ2.GameContent.Damages;
using MVZ2.GameContent.Effects;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Level;
using MVZ2.Vanilla.Properties;
using MVZ2.Vanilla.Shells;
using MVZ2Logic.Level;
using PVZEngine;
using PVZEngine.Damages;
using PVZEngine.Entities;
using PVZEngine.Level;

namespace MVZ2.GameContent.Projectiles
{
    [EntityBehaviourDefinition(VanillaProjectileNames.soulfireBall)]
    public class SoulfireBall : ProjectileBehaviour
    {
        public SoulfireBall(string nsp, string name) : base(nsp, name)
        {
        }
        protected override void PostHitEntity(ProjectileHitOutput hitResult, DamageOutput damageOutput)
        {
            base.PostHitEntity(hitResult, damageOutput);
            if (damageOutput == null)
                return;
            var entity = hitResult.Projectile;
            var other = hitResult.Other;

            bool blocksFire = damageOutput.WillDamageBlockFire();

            var blast = IsBlast(entity);
            if (blast)
            {
                entity.PlaySound(VanillaSoundID.darkSkiesImpact);
                entity.Level.ShakeScreen(3, 3, 3);
                entity.Level.Spawn(VanillaEffectID.soulfireBlast, entity.Position, entity);
            }
            else if (!blocksFire)
            {
                entity.Level.Spawn(VanillaEffectID.soulfire, entity.Position, entity);
            }

            if (!blocksFire || blast)
            {
                var damageEffects = new DamageEffectList(VanillaDamageEffects.FIRE, VanillaDamageEffects.EXPLOSION, VanillaDamageEffects.MUTE);
                entity.Level.Explode(entity.Position, 40, entity.GetFaction(), entity.GetDamage() / 3f, damageEffects, entity);
            }
        }
        public static void SetBlast(Entity entity, bool value)
        {
            entity.SetBehaviourField(ID, PROP_BLAST, value);
        }
        public static bool IsBlast(Entity entity)
        {
            return entity.GetBehaviourField<bool>(ID, PROP_BLAST);
        }
        public static NamespaceID ID => VanillaProjectileID.soulfireBall;
        public static readonly VanillaEntityPropertyMeta PROP_BLAST = new VanillaEntityPropertyMeta("Blast");
    }
}
