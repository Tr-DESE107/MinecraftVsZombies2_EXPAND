using MVZ2.GameContent.Damages;
using MVZ2.GameContent.Effects;
using MVZ2.Vanilla;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Level;
using MVZ2.Vanilla.Shells;
using MVZ2Logic.Level;
using PVZEngine;
using PVZEngine.Damages;
using PVZEngine.Entities;

namespace MVZ2.GameContent.Projectiles
{
    [Definition(VanillaProjectileNames.soulfireBall)]
    public class SoulfireBall : ProjectileBehaviour
    {
        public SoulfireBall(string nsp, string name) : base(nsp, name)
        {
        }
        protected override void PostHitEntity(Entity entity, Entity other)
        {
            base.PostHitEntity(entity, other);
            var blocksFire = other.GetShellDefinition()?.BlocksFire() ?? false;
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
                entity.Level.Explode(entity.Position, 40, entity.GetFaction(), entity.GetDamage() / 3f, damageEffects, new EntityReferenceChain(entity));
            }
        }
        public static void SetBlast(Entity entity, bool value)
        {
            entity.SetBehaviourProperty(ID, "Blast", value);
        }
        public static bool IsBlast(Entity entity)
        {
            return entity.GetBehaviourProperty<bool>(ID, "Blast");
        }
        public static NamespaceID ID => VanillaProjectileID.soulfireBall;
    }
}
