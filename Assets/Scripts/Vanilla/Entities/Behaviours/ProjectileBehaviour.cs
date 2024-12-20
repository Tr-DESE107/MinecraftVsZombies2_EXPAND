using System;
using MVZ2.Vanilla.Callbacks;
using MVZ2.Vanilla.Level;
using PVZEngine;
using PVZEngine.Damages;
using PVZEngine.Entities;

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
            entity.Timeout = entity.GetMaxTimeout();
            entity.CollisionMaskHostile = EntityCollisionHelper.MASK_PLANT
                | EntityCollisionHelper.MASK_ENEMY
                | EntityCollisionHelper.MASK_OBSTACLE
                | EntityCollisionHelper.MASK_BOSS;
            entity.UpdatePointTowardsDirection();
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
            projectile.UpdatePointTowardsDirection();
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
            return bounds.max.x < VanillaLevelExt.PROJECTILE_LEFT_BORDER ||
                bounds.min.x > VanillaLevelExt.PROJECTILE_RIGHT_BORDER ||
                position.z > VanillaLevelExt.PROJECTILE_UP_BORDER ||
                position.z < VanillaLevelExt.PROJECTILE_DOWN_BORDER ||
                position.y > VanillaLevelExt.PROJECTILE_TOP_BORDER ||
                position.y < VanillaLevelExt.PROJECTILE_BOTTOM_BORDER;
        }
        protected virtual void PreHitEntity(ProjectileHitInput hit, DamageInput damage)
        {
        }
        protected virtual void PostHitEntity(ProjectileHitOutput hitResult, DamageOutput damage)
        {
        }
        private void UnitCollide(EntityCollision collision)
        {
            var projectile = collision.Entity;
            if (projectile.Removed)
                return;

            // 不能击中死亡的实体。
            var other = collision.Other;
            if (other.IsDead)
                return;

            // 不能击中发射者。
            var spawner = projectile.SpawnerReference?.GetEntity(projectile.Level);
            if (other == spawner && !projectile.CanHitSpawner())
                return;

            // 不是敌方
            if (!projectile.IsHostile(other))
                return;

            // 已经击中过对方
            var otherCollider = collision.OtherCollider;
            var collided = projectile.GetProjectileCollidingColliders();
            var otherReference = otherCollider.ToReference();
            if (collided != null && collided.Contains(otherReference))
                return;


            // 击中敌人前
            var hitInput = new ProjectileHitInput()
            {
                Projectile = projectile,
                Other = other,
                Pierce = projectile.IsPiercing()
            };
            var damageEffects = projectile.GetDamageEffects();
            DamageEffectList effects = new DamageEffectList(damageEffects ?? Array.Empty<NamespaceID>());
            var damageInput = otherCollider.GetDamageInput(projectile.GetDamage(), effects, projectile);
            if (damageInput == null)
                return;

            // 触发击中前回调。
            PreHitEntity(hitInput, damageInput);
            if (hitInput.Canceled)
                return;
            var filterValue = projectile.GetDefinitionID();
            foreach (var trigger in projectile.Level.Triggers.GetTriggers(VanillaLevelCallbacks.PRE_PROJECTILE_HIT))
            {
                if (!trigger.Filter(filterValue))
                    continue;
                trigger.Run(hitInput, damageInput);
                if (hitInput.Canceled)
                    return;
            }

            // 对敌人造成伤害
            DamageOutput damageOutput = VanillaEntityExt.TakeDamage(damageInput);

            // 击中敌人后
            ProjectileHitOutput hitOutput = new ProjectileHitOutput()
            {
                Projectile = hitInput.Projectile,
                Other = hitInput.Other,
                Pierce = hitInput.Pierce
            };
            if (damageOutput != null)
            {
                if (damageOutput.ShieldResult != null)
                {
                    hitOutput.Shield = damageOutput.ShieldResult.Armor;
                }
                else
                {
                    bool ethereal = damageOutput.ArmorResult != null ? false : other.IsEthereal();
                    hitOutput.Pierce = ethereal || hitOutput.Pierce;
                }
            }
            // 将碰撞箱加入到已被碰撞的的列表。
            projectile.AddProjectileCollidingEntity(otherReference);

            // 触发击中后回调。
            PostHitEntity(hitOutput, damageOutput);
            projectile.Level.Triggers.RunCallbackFiltered(VanillaLevelCallbacks.POST_PROJECTILE_HIT, projectile.GetDefinitionID(), hitOutput, damageOutput);

            if (!hitOutput.Pierce)
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