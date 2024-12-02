using System;
using System.Linq;
using MVZ2.Vanilla.Callbacks;
using MVZ2.Vanilla.Detections;
using MVZ2Logic;
using PVZEngine;
using PVZEngine.Armors;
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
            entity.CollisionMaskHostile = EntityCollisionHelper.MASK_PLANT
                | EntityCollisionHelper.MASK_ENEMY
                | EntityCollisionHelper.MASK_OBSTACLE
                | EntityCollisionHelper.MASK_BOSS;
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
        public override void PostCollision(EntityCollision collision, int state)
        {
            base.PostCollision(collision, state);
            var entity = collision.Entity;
            if (entity.GetProperty<bool>(VanillaProjectileProps.NO_HIT_ENTITIES))
                return;
            if (state == EntityCollisionHelper.STATE_EXIT)
            {
                UnitExit(collision);
                var spawner = entity.SpawnerReference?.GetEntity(entity.Level);
                if (collision.Other == spawner)
                {
                    entity.SetCanHitSpawner(true);
                }
            }
            else
            {
                UnitCollide(collision);
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
        protected virtual void PostHitEntity(ProjectileHitOutput hitResult, DamageOutput result)
        {
        }
        private void UnitCollide(EntityCollision collision)
        {
            var projectile = collision.Entity;
            if (projectile.Removed)
                return;
            var other = collision.Other;
            if (other.IsDead)
                return;
            // 是否可以击中发射者。
            var spawner = projectile.SpawnerReference?.GetEntity(projectile.Level);
            if (other == spawner && !projectile.CanHitSpawner())
                return;
            if (!projectile.IsHostile(other))
                return;
            var otherCollider = collision.OtherCollider;
            var collided = projectile.GetProjectileCollidingColliders();
            var otherReference = otherCollider.ToReference();
            if (collided != null && collided.Contains(otherReference))
                return;

            bool canPierce = false;
            var damageEffects = projectile.GetDamageEffects();
            DamageEffectList effects = new DamageEffectList(damageEffects ?? Array.Empty<NamespaceID>());
            DamageOutput damageResult = otherCollider.TakeDamage(projectile.GetDamage(), effects, projectile);

            ProjectileHitOutput hitResult = new ProjectileHitOutput()
            {
                Projectile = projectile,
                Other = other,
                Pierce = canPierce
            };
            if (damageResult.ShieldResult != null)
            {
                hitResult.Shield = damageResult.ShieldResult.Armor;
                canPierce = projectile.IsPiercing();
            }
            else
            {
                bool ethereal = damageResult.ArmorResult != null ? false : other.IsEthereal();
                canPierce = ethereal || projectile.IsPiercing();
            }

            projectile.AddProjectileCollidingEntity(otherReference);
            PostHitEntity(hitResult, damageResult);
            Global.Game.RunCallbackFiltered(VanillaLevelCallbacks.POST_PROJECTILE_HIT, projectile.GetDefinitionID(), hitResult, damageResult);

            if (!hitResult.Pierce)
            {
                projectile.Remove();
                return;
            }
        }
        private void UnitExit(EntityCollision collision)
        {
            collision.Entity.RemoveProjectileCollidingEntity(collision.OtherCollider.ToReference());
        }
    }
}