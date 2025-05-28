using MVZ2.GameContent.Buffs;
using MVZ2.GameContent.Damages;
using MVZ2.Vanilla.Callbacks;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Shells;
using MVZ2Logic.Modding;
using PVZEngine;
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
            mod.AddTrigger(VanillaLevelCallbacks.POST_ENTITY_TAKE_DAMAGE, DamageEffectCallback);
            mod.AddTrigger(LevelCallbacks.POST_ENTITY_UPDATE, HealParticlesUpdateCallback);
            mod.AddTrigger(LevelCallbacks.POST_ENTITY_DEATH, PostEnemyDeathCallback, filter: EntityTypes.ENEMY);
            mod.AddTrigger(LevelCallbacks.POST_DESTROY_ARMOR, PostArmorDestroyCallback);
        }
        private void PostEntityInitCallback(EntityCallbackParams param, CallbackResult result)
        {
            var entity = param.entity;
            entity.AddBuff<EntityPhysicsBuff>();
            if (entity.IsVulnerableEntity())
            {
                entity.AddBuff<FactionBuff>();
            }
        }
        private void PostContactGroundCallback(LevelCallbacks.PostEntityContactGroundParams param, CallbackResult result)
        {
            var entity = param.entity;
            var velocity = param.velocity;
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
        private void PlayHitSoundCallback(VanillaLevelCallbacks.PostTakeDamageParams param, CallbackResult callbackResult)
        {
            var output = param.output;
            var bodyResult = output.BodyResult;
            var entity = output.Entity;
            if (bodyResult != null)
            {
                var shellDefinition = bodyResult.ShellDefinition;
                if (bodyResult.Effects.HasEffect(VanillaDamageEffects.SLICE) && shellDefinition.IsSliceCritical())
                {
                    entity.EmitBlood();
                }
            }
            output.PlayHitSound();
        }
        private void DamageEffectCallback(VanillaLevelCallbacks.PostTakeDamageParams param, CallbackResult callbackResult)
        {
            var output = param.output;
            var entity = output.Entity;
            if (entity.Type != EntityTypes.ENEMY)
                return;
            var bodyResult = output.BodyResult;
            var armorResult = output.ArmorResult;
            bool slow = false;
            bool unfreeze = false;
            if (bodyResult != null)
            {
                if (bodyResult.HasEffect(VanillaDamageEffects.SLOW))
                {
                    slow = true;
                }
                if (bodyResult.HasEffect(VanillaDamageEffects.FIRE))
                {
                    unfreeze = true;
                }
            }
            if (armorResult != null)
            {
                if (armorResult.HasEffect(VanillaDamageEffects.SLOW))
                {
                    slow = true;
                }
                if (armorResult.HasEffect(VanillaDamageEffects.FIRE))
                {
                    unfreeze = true;
                }
            }
            if (unfreeze)
            {
                entity.Unfreeze();
            }
            else if (slow)
            {
                entity.Slow(300);
            }
        }
        private void PostEnemyDeathCallback(LevelCallbacks.PostEntityDeathParams param, CallbackResult result)
        {
            var entity = param.entity;
            var info = param.deathInfo;
            if (info.HasEffect(VanillaDamageEffects.NO_NEUTRALIZE))
                return;
            entity.Neutralize();
        }
        private void HealParticlesUpdateCallback(EntityCallbackParams param, CallbackResult result)
        {
            var entity = param.entity;
            entity.UpdateHealParticles();
        }
        private void PostArmorDestroyCallback(LevelCallbacks.PostArmorDestroyParams param, CallbackResult result)
        {
            var armor = param.armor;
            var entity = param.entity;
            var deathSound = armor.GetDeathSound();
            if (!NamespaceID.IsValid(deathSound))
                return;
            entity.PlaySound(deathSound);
        }
    }
}
