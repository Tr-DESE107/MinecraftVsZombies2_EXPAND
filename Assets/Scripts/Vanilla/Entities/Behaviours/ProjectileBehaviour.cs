using System;
using System.Linq;
using MVZ2.Vanilla.Callbacks;
using MVZ2.Vanilla.Detections;
using MVZ2Logic;
using PVZEngine;
using PVZEngine.Damages;
using PVZEngine.Entities;
using UnityEngine;

namespace MVZ2.Vanilla.Entities
{
    public abstract class ProjectileBehaviour : VanillaEntityBehaviour
    {
        protected ProjectileBehaviour(string nsp, string name) : base(nsp, name)
        {
        }

        public override void Init(Entity entity)
        {
            base.Init(entity);
            entity.SetShadowScale(new Vector3(0.5f, 0.5f, 1));
            entity.Timeout = entity.GetMaxTimeout();
            entity.CollisionMask = EntityCollision.MASK_PLANT
                | EntityCollision.MASK_ENEMY
                | EntityCollision.MASK_OBSTACLE
                | EntityCollision.MASK_BOSS;
            if (entity.PointsTowardDirection())
            {
                var vel = new Vector2(entity.Velocity.x, entity.Velocity.y + entity.Velocity.z);
                entity.RenderRotation = Vector3.forward * Vector2.SignedAngle(Vector2.right, vel);
            }
        }

        public override void Update(Entity projectile)
        {
            base.Update(projectile);
            projectile.Timeout--;
            if (projectile.Timeout <= 0)
            {
                projectile.Remove();
                return;
            }
            if (projectile.WillDestroyOutsideLawn() && IsOutsideView(projectile))
            {
                projectile.Remove();
                return;
            }
            if (projectile.PointsTowardDirection())
            {
                var vel = new Vector2(projectile.Velocity.x, projectile.Velocity.y + projectile.Velocity.z);
                projectile.RenderRotation = Vector3.forward * Vector2.SignedAngle(Vector2.right, vel);
            }
        }
        public override void PostCollision(Entity entity, Entity other, int state)
        {
            base.PostCollision(entity, other, state);
            if (entity.GetProperty<bool>(VanillaProjectileProps.NO_HIT_ENTITIES))
                return;
            if (state != EntityCollision.STATE_EXIT)
            {
                UnitCollide(entity, other);
            }
            else
            {
                UnitExit(entity, other);
                var spawner = entity.SpawnerReference?.GetEntity(entity.Level);
                if (other == spawner)
                {
                    entity.SetCanHitSpawner(true);
                }
            }
        }

        public override void PostStopChangingLane(Entity entity)
        {
            base.PostStopChangingLane(entity);
            var vel = entity.Velocity;
            vel.z = 0;
            entity.Velocity = vel;
        }
        public virtual bool IsOutsideView(Entity proj)
        {
            var bounds = proj.GetBounds();
            var position = proj.Position;
            return position.x < 180 - bounds.extents.x ||
                position.x > 1100 + bounds.extents.x ||
                position.z > 550 ||
                position.z < -50 ||
                position.y > 1000 ||
                position.y < -1000;
        }
        protected virtual void PostHitEntity(Entity entity, Entity other)
        {

        }
        private void UnitCollide(Entity entity, Entity other)
        {
            // 是否可以击中发射者。
            var spawner = entity.SpawnerReference?.GetEntity(entity.Level);
            if (other == spawner && !entity.CanHitSpawner())
                return;

            var collided = entity.GetProjectileCollidingEntities();
            if (entity.Removed || !entity.IsHostile(other) || collided != null && collided.Any(c => c.ID == other.ID) || other.IsDead)
                return;

            if (!Detection.IsZCoincide(entity.Position.z, entity.GetScaledSize().z, other.Position.z, other.GetScaledSize().z))
                return;

            var damageEffects = entity.GetDamageEffects();
            DamageEffectList effects = new DamageEffectList(damageEffects ?? Array.Empty<NamespaceID>());
            other.TakeDamage(entity.GetDamage(), effects, new EntityReferenceChain(entity));

            entity.AddProjectileCollidingEntity(other);

            PostHitEntity(entity, other);
            Global.Game.RunCallbackFiltered(VanillaLevelCallbacks.POST_PROJECTILE_HIT, entity.GetDefinitionID(), entity, other);

            if (!entity.CanPierce(other))
            {
                entity.Remove();
                return;
            }
        }
        private void UnitExit(Entity entity, Entity other)
        {
            entity.RemoveProjectileCollidingEntity(other);
        }
    }
}