using MVZ2.GameContent.Buffs;
using MVZ2.GameContent.Damages;
using MVZ2.Vanilla.Callbacks;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Shells;
using MVZ2Logic.Modding;
using PVZEngine.Callbacks;
using PVZEngine.Damages;
using PVZEngine.Entities;
using UnityEngine;

namespace MVZ2.GameContent.Implements
{
    public class EntityImplements : VanillaImplements
    {
        public override void Implement(Mod mod)
        {
            mod.AddTrigger(LevelCallbacks.POST_ENTITY_INIT, PostEntityInitCallback);
            mod.AddTrigger(LevelCallbacks.POST_ENTITY_CONTACT_GROUND, PostContactGroundCallback);
            mod.AddTrigger(VanillaLevelCallbacks.POST_ENTITY_TAKE_DAMAGE, PlayHitSoundCallback);
            mod.AddTrigger(LevelCallbacks.POST_ENTITY_UPDATE, HealParticlesUpdateCallback);
            mod.AddTrigger(LevelCallbacks.POST_ENTITY_DEATH, PostEnemyDeathCallback, filter: EntityTypes.ENEMY);
        }
        private void PostEntityInitCallback(Entity entity)
        {
            entity.AddBuff<EntityPhysicsBuff>();
            if (entity.IsVulnerableEntity())
            {
                entity.AddBuff<FactionBuff>();
            }
        }
        private void PostContactGroundCallback(Entity entity, Vector3 velocity)
        {
            if (!entity.IsVulnerableEntity())
                return;
            if (entity.IsOnWater())
                return;
            if (velocity.y >= 0)
                return;

            var damageThresold = -entity.GetFallResistance();
            if (velocity.y > damageThresold)
                return;
            var fallDamage = 2.5f * Mathf.Pow(velocity.y - damageThresold, 2);
            if (fallDamage > 0)
            {
                var effects = new DamageEffectList(VanillaDamageEffects.IGNORE_ARMOR, VanillaDamageEffects.FALL_DAMAGE);
                entity.TakeDamage(fallDamage, effects, entity);
            }
        }
        private void PlayHitSoundCallback(DamageOutput result)
        {
            var bodyResult = result.BodyResult;
            var armorResult = result.ArmorResult;
            var entity = result.Entity;
            if (bodyResult != null)
            {
                var shellDefinition = bodyResult.ShellDefinition;
                if (bodyResult.Effects.HasEffect(VanillaDamageEffects.SLICE) && shellDefinition.IsSliceCritical())
                {
                    entity.EmitBlood();
                }
            }
            result.PlayHitSound();
        }
        private void PostEnemyDeathCallback(Entity entity, DeathInfo damage)
        {
            if (damage.HasEffect(VanillaDamageEffects.NO_NEUTRALIZE))
                return;
            entity.Neutralize();
        }
        private void HealParticlesUpdateCallback(Entity entity)
        {
            entity.UpdateHealParticles();
        }
    }
}
