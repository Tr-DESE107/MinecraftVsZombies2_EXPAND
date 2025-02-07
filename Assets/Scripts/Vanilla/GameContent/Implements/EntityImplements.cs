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
            mod.AddTrigger(LevelCallbacks.POST_ENTITY_UPDATE, ChangeLaneUpdateCallback);
            mod.AddTrigger(LevelCallbacks.POST_ENTITY_UPDATE, HealParticlesUpdateCallback);
            mod.AddTrigger(LevelCallbacks.POST_ENTITY_DEATH, PostEnemyDeathCallback, filter: EntityTypes.ENEMY);
        }
        private void PostEntityInitCallback(Entity entity)
        {
            entity.AddBuff<EntityPhysicsBuff>();
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
            var fallDamage = Mathf.Pow(velocity.y - damageThresold, 2);
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
        private void ChangeLaneUpdateCallback(Entity entity)
        {
            var changeLane = entity.Definition.GetBehaviour<IChangeLaneEntity>();
            if (changeLane == null)
                return;
            if (!changeLane.IsChangingLane(entity))
                return;
            var targetLane = changeLane.GetChangeLaneTarget(entity);
            if (targetLane < 0 || targetLane > entity.Level.GetMaxLaneCount())
                return;
            var sourceLane = changeLane.GetChangeLaneSource(entity);

            float targetZ = entity.Level.GetEntityLaneZ(targetLane);
            bool passed;
            // Warp upwards.
            if (sourceLane > targetLane)
            {
                passed = entity.Position.z >= targetZ - 0.03f;
            }
            // Warp downwards.
            else
            {
                passed = entity.Position.z <= targetZ + 0.03f;
            }

            Vector3 velocity = entity.Velocity;
            if (!passed)
            {
                float warpSpeed = changeLane.GetChangeLaneSpeed(entity);

                // Warp upwards.
                if (sourceLane > targetLane)
                {
                    velocity.z = Mathf.Max(warpSpeed, entity.Velocity.z);
                }
                // Warp downwards.
                else
                {
                    velocity.z = Mathf.Min(-warpSpeed, entity.Velocity.z);
                }
            }
            else
            {
                if (Mathf.Abs(entity.Position.z - targetZ) <= 0.05f)
                {
                    var pos = entity.Position;
                    pos.z = targetZ;
                    entity.Position = pos;
                }
                entity.StopChangingLane();

                velocity.z = 0;
            }
            entity.Velocity = velocity;
        }
        private void PostEnemyDeathCallback(Entity entity, DeathInfo damage)
        {
            entity.Neutralize();
        }
        private void HealParticlesUpdateCallback(Entity entity)
        {
            entity.UpdateHealParticles();
        }
    }
}
