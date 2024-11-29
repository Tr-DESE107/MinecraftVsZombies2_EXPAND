using MVZ2.GameContent.Buffs;
using MVZ2.GameContent.Damages;
using MVZ2.GameContent.Projectiles;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Callbacks;
using MVZ2.Vanilla.Entities;
using MVZ2Logic.Level;
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
        }
        private void PostEntityInitCallback(Entity entity)
        {
            entity.AddBuff<EntityPhysicsBuff>();
        }
        private void PostContactGroundCallback(Entity entity, Vector3 velocity)
        {
            if (!EntityTypes.IsDamagable(entity.Type))
                return;
            float fallHeight = Mathf.Max(0, entity.GetFallDamage() - velocity.y * 5);
            float fallDamage = Mathf.Pow(fallHeight, 2);
            if (fallDamage > 0)
            {
                var effects = new DamageEffectList(VanillaDamageEffects.IGNORE_ARMOR, VanillaDamageEffects.FALL_DAMAGE);
                entity.TakeDamage(fallDamage, effects, new EntityReferenceChain(null));
            }
        }
        private void PlayHitSoundCallback(DamageResult bodyResult, DamageResult armorResult)
        {
            if (armorResult != null && !armorResult.Effects.HasEffect(VanillaDamageEffects.MUTE))
            {
                var entity = armorResult.Entity;
                var shellDefinition = armorResult.ShellDefinition;
                entity.PlayHitSound(armorResult.Effects, shellDefinition);
            }
            if (bodyResult != null && !bodyResult.Effects.HasEffect(VanillaDamageEffects.MUTE))
            {
                var entity = bodyResult.Entity;
                var shellDefinition = bodyResult.ShellDefinition;
                entity.PlayHitSound(bodyResult.Effects, shellDefinition);
            }
        }
        private void ChangeLaneUpdateCallback(Entity entity)
        {
            if (entity.Definition is not IChangeLaneEntity changeLane)
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

            if (!passed)
            {
                Vector3 velocity = entity.Velocity;
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
                entity.Velocity = velocity;
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
            }
        }
    }
}
