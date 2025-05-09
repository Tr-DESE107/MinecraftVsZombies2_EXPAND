using MVZ2.GameContent.Damages;
using MVZ2.GameContent.Effects;
using MVZ2.GameContent.Fragments;
using MVZ2.GameContent.Projectiles;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Callbacks;
using MVZ2.Vanilla.Contraptions;
using MVZ2.Vanilla.Entities;
using PVZEngine;
using PVZEngine.Armors;
using PVZEngine.Callbacks;
using PVZEngine.Damages;
using PVZEngine.Entities;
using PVZEngine.Level;
using UnityEngine;

namespace MVZ2.GameContent.Armors
{
    [ArmorBehaviourDefinition(VanillaArmorBehaviourNames.reflectiveBarrier)]
    public class ReflectiveBarrier : ArmorBehaviourDefinition
    {
        public ReflectiveBarrier(string nsp, string name) : base(nsp, name)
        {
            AddTrigger(LevelCallbacks.POST_ENTITY_DEATH, PostEntityDeathCallback);
            AddTrigger(LevelCallbacks.POST_DESTROY_ARMOR, PostArmorDestroyCallback);
            AddTrigger(VanillaLevelCallbacks.POST_PROJECTILE_HIT, PostProjectileHitCallback);
        }
        private void PostEntityDeathCallback(Entity entity, DeathInfo info)
        {
            if (info.HasEffect(VanillaDamageEffects.REMOVE_ON_DEATH))
                return;
            var shield = entity.GetArmorAtSlot(VanillaArmorSlots.shield);
            if (shield == null)
                return;
            if (!shield.Definition.HasBehaviour(this))
                return;
            shield.Destroy();
        }
        private void PostArmorDestroyCallback(Entity entity, NamespaceID slot, Armor armor, ArmorDestroyInfo info)
        {
            if (!armor.Definition.HasBehaviour(this))
                return;
            var pos = entity.Position + new Vector3(entity.GetFacingX() * 20, 40, 0);
            var fragment = entity.Level.Spawn(VanillaEffectID.fragment, pos, entity);
            Fragment.SetFragmentID(fragment, VanillaFragmentID.reflectiveBarrier);
            Fragment.AddEmitSpeed(fragment, 500);
        }
        private void PostProjectileHitCallback(ProjectileHitOutput hit, DamageOutput damage)
        {
            if (hit.Pierce)
                return;

            var shield = hit.Shield;
            if (shield == null)
                return;
            var shieldResult = damage.ShieldResult;
            if (shieldResult == null)
                return;
            if (!shield.Definition.HasBehaviour(this))
                return;

            var owner = shield.Owner;
            var shootParams = owner.GetShootParams();
            shootParams.projectileID = VanillaProjectileID.reflectionBullet;
            shootParams.position = hit.Projectile.Position;
            shootParams.damage = shieldResult.Amount;
            shootParams.soundID = VanillaSoundID.reflection;
            shootParams.velocity = owner.GetFacingDirection() * 10;
            owner.ShootProjectile(shootParams);
        }
    }
}
