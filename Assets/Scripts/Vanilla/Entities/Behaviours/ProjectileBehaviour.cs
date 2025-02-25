using System;
using MVZ2.GameContent.Damages;
using MVZ2.Vanilla.Callbacks;
using MVZ2.Vanilla.Level;
using MVZ2.Vanilla.Properties;
using PVZEngine;
using PVZEngine.Damages;
using PVZEngine.Entities;
using PVZEngine.Triggers;
using Tools;
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
            entity.Timeout = entity.GetMaxTimeout();
            entity.CollisionMaskHostile = EntityCollisionHelper.MASK_PLANT
                | EntityCollisionHelper.MASK_ENEMY
                | EntityCollisionHelper.MASK_OBSTACLE
                | EntityCollisionHelper.MASK_BOSS;
            entity.UpdatePointTowardsDirection();
            SetStartHitSpawnerProtectTimeout(entity, 2);
        }

        public override void Update(Entity projectile)
        {
            base.Update(projectile);
            projectile.Timeout--;
            if (projectile.Timeout <= 0)
            {
                projectile.Die();
                return;
            }
            if (projectile.WillDestroyOutsideLawn() && IsOutsideView(projectile))
            {
                projectile.Remove();
                return;
            }
            projectile.UpdatePointTowardsDirection();

            var protectTimout = GetStartHitSpawnerProtectTimeout(projectile);
            if (protectTimout > 0)
            {
                protectTimout--;
                SetStartHitSpawnerProtectTimeout(projectile, protectTimout);
            }

            RollUpdate(projectile);
        }
        public override void PostCollision(EntityCollision collision, int state)
        {
            base.PostCollision(collision, state);
            var entity = collision.Entity;
            if (entity.GetProperty<bool>(VanillaProjectileProps.NO_HIT_ENTITIES))
                return;
            var spawner = entity.SpawnerReference?.GetEntity(entity.Level);
            if (state == EntityCollisionHelper.STATE_EXIT)
            {
                if (collision.Other == spawner && HasHitSpawnerProtect(entity))
                {
                    SetHitSpawnerProtect(entity, false);
                }
                UnitExit(collision);
            }
            else
            {
                if (collision.Other == spawner)
                {
                    if (GetStartHitSpawnerProtectTimeout(entity) > 0)
                    {
                        SetHitSpawnerProtect(entity, true);
                        SetStartHitSpawnerProtectTimeout(entity, 0);
                    }
                    if (HasHitSpawnerProtect(entity))
                    {
                        return;
                    }
                }
                UnitCollide(collision);
            }
        }

        public override void PostContactGround(Entity entity, Vector3 velocity)
        {
            base.PostContactGround(entity, velocity);
            if (entity.KillOnGround())
            {
                entity.Die();
            }
        }
        public override void PostDeath(Entity entity, DeathInfo damageInfo)
        {
            base.PostDeath(entity, damageInfo);
            if (damageInfo.Effects.HasEffect(VanillaDamageEffects.DROWN))
            {
                entity.PlaySplashEffect();
                entity.PlaySplashSound();
            }
            entity.Remove();
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
            if (hitInput.IsInterrupted)
                return;
            var filterValue = projectile.GetDefinitionID();
            projectile.Level.Triggers.RunCallbackFiltered(VanillaLevelCallbacks.PRE_PROJECTILE_HIT, filterValue, hitInput, c => c(hitInput, damageInput));
            if (hitInput.IsInterrupted)
                return;

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
            projectile.Level.Triggers.RunCallbackFiltered(VanillaLevelCallbacks.POST_PROJECTILE_HIT, projectile.GetDefinitionID(), c => c(hitOutput, damageOutput));

            if (!hitOutput.Pierce)
            {
                projectile.Die();
                return;
            }
        }
        private void UnitExit(EntityCollision collision)
        {
            var entity = collision.Entity;
            entity.RemoveProjectileCollidingEntity(collision.OtherCollider.ToReference());
        }
        private void RollUpdate(Entity projectile)
        {
            if (!projectile.Rolls())
                return;
            var gravity = projectile.GetGravity();
            if (gravity > 0 && projectile.GetRelativeY() <= 0)
            {
                var level = projectile.Level;
                var x = projectile.Position.x;
                var z = projectile.Position.z;
                var groundY = projectile.GetGroundY();
                var velocityAddition = Vector2.zero;
                var checkDistance = 1;
                for (int i = 0; i < 8; i++)
                {
                    var direction = Vector2.right.RotateClockwise(i * 45);
                    var opposite = -direction;
                    var checkPoint = direction * checkDistance;
                    var relativeGroundY = level.GetGroundY(x + checkPoint.x, z + checkPoint.y) - groundY;

                    var slope = Mathf.Atan2(relativeGroundY, checkDistance);

                    var horiSpeed = gravity * Mathf.Sin(slope) * Mathf.Cos(slope);
                    velocityAddition += opposite * horiSpeed;
                }
                var vel = projectile.Velocity;
                vel.x += velocityAddition.x;
                vel.z += velocityAddition.y;
                projectile.Velocity = vel;
            }
        }

        #region 创建者保护
        private void SetStartHitSpawnerProtectTimeout(Entity entity, int value) => entity.SetBehaviourField(FIELD_HIT_SPAWNER_PROTECT_TIMEOUT, value);
        private int GetStartHitSpawnerProtectTimeout(Entity entity) => entity.GetBehaviourField<int>(FIELD_HIT_SPAWNER_PROTECT_TIMEOUT);
        private void SetHitSpawnerProtect(Entity entity, bool value) => entity.SetBehaviourField(FIELD_HIT_SPAWNER_PROTECT, value);
        private bool HasHitSpawnerProtect(Entity entity) => entity.GetBehaviourField<bool>(FIELD_HIT_SPAWNER_PROTECT);
        #endregion
        private const string PROP_REGION = "projectile_behaviour";
        [PropertyRegistry(PROP_REGION)]
        public static readonly VanillaEntityPropertyMeta FIELD_HIT_SPAWNER_PROTECT_TIMEOUT = new VanillaEntityPropertyMeta("HitSpawnerProtectTimeout");
        [PropertyRegistry(PROP_REGION)]
        public static readonly VanillaEntityPropertyMeta FIELD_HIT_SPAWNER_PROTECT = new VanillaEntityPropertyMeta("HitSpawnerProtect");
    }
}