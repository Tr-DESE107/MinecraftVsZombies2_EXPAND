using System;
using MVZ2.GameContent.Buffs;
using MVZ2.GameContent.Buffs.Enemies;
using MVZ2.GameContent.Damages;
using MVZ2.Vanilla.Level;
using PVZEngine.Damages;
using PVZEngine.Entities;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

namespace MVZ2.Vanilla.Entities
{
    public abstract class EnemyBehaviour : VanillaEntityBehaviour
    {
        public EnemyBehaviour(string nsp, string name) : base(nsp, name)
        {
        }
        public override void Init(Entity entity)
        {
            base.Init(entity);

            var buff = entity.CreateBuff<RandomEnemySpeedBuff>();
            buff.SetProperty(RandomEnemySpeedBuff.PROP_SPEED, GetRandomSpeedMultiplier(entity));
            entity.AddBuff(buff);

            entity.SetFaction(entity.Level.Option.RightFaction);

            entity.CollisionMaskHostile = EntityCollisionHelper.MASK_PLANT
                | EntityCollisionHelper.MASK_ENEMY
                | EntityCollisionHelper.MASK_OBSTACLE
                | EntityCollisionHelper.MASK_BOSS;
        }
        public override sealed void Update(Entity entity)
        {
            base.Update(entity);
            if (!entity.IsAIFrozen())
            {
                UpdateAI(entity);
            }
            UpdateLogic(entity);
        }
        protected virtual void UpdateLogic(Entity entity)
        {
            if (entity.IsFriendlyEntity() || !entity.IsFacingLeft())
            {
                if (IsOutsideView(entity))
                {
                    entity.Neutralize();
                    entity.Remove();
                    return;
                }
            }
            else
            {
                Vector3 pos = entity.Position;
                pos.x = Mathf.Min(pos.x, VanillaLevelExt.GetEnemyRightBorderX());
                entity.Position = pos;
            }
            ChangeLaneUpdate(entity);
            var scale = entity.GetFinalDisplayScale();
            var scaleX = Mathf.Abs(scale.x);
            var attackSpeed = entity.GetAttackSpeed() / entity.GetAttackSpeed(ignoreBuffs: true) / scaleX;
            var speed = entity.GetSpeed() / entity.GetSpeed(ignoreBuffs: true) / scaleX;
            if (entity.IsAIFrozen())
            {
                attackSpeed = 0;
                speed = 0;
            }
            entity.SetAnimationFloat("AttackSpeed", attackSpeed);
            entity.SetAnimationFloat("MoveSpeed", speed);
        }
        protected virtual void UpdateAI(Entity entity)
        {
        }
        public override void PostTakeDamage(DamageOutput result)
        {
            base.PostTakeDamage(result);
            var bodyResult = result.BodyResult;
            if (bodyResult != null && bodyResult.Amount > 0)
            {
                var entity = bodyResult.Entity;
                if (!entity.HasBuff<DamageColorBuff>())
                    entity.AddBuff<DamageColorBuff>();
            }
        }
        public override void PostDeath(Entity entity, DeathInfo damageInfo)
        {
            base.PostDeath(entity, damageInfo);
            if (damageInfo.Effects.HasEffect(VanillaDamageEffects.REMOVE_ON_DEATH))
            {
                entity.Remove();
                return;
            }
            if (!entity.HasBuff<DamageColorBuff>())
                entity.AddBuff<DamageColorBuff>();
            entity.PlaySound(entity.GetDeathSound(), entity.GetCryPitch());
        }
        protected virtual bool IsOutsideView(Entity enemy)
        {
            var bounds = enemy.GetBounds();
            var position = enemy.Position;
            return bounds.max.x < VanillaLevelExt.ENEMY_LEFT_BORDER ||
                bounds.min.x > VanillaLevelExt.ENEMY_RIGHT_BORDER;
        }
        protected virtual float GetRandomSpeedMultiplier(Entity entity)
        {
            return entity.RNG.Next(1, 1.5f);
        }
        private void ChangeLaneUpdate(Entity entity)
        {
            if (!entity.IsOnGround)
                return;
            var level = entity.Level;
            var minLane = 0;
            var maxLane = level.GetMaxLaneCount() - 1;
            var lane = Mathf.Clamp(entity.GetLane(), minLane, maxLane);
            var targetZ = level.GetEntityLaneZ(lane);
            var targetZDistance = entity.Position.z - targetZ;
            if (Mathf.Abs(targetZDistance) >= CHANGE_LANE_THRESOLD && !entity.IsChangingLane())
            {
                int targetLane;
                int adjacentLane = lane - Math.Sign(targetZDistance);
                if (adjacentLane >= minLane && adjacentLane <= maxLane)
                {
                    var adjacentZ = level.GetEntityLaneZ(adjacentLane);
                    var adjacentZDistance = entity.Position.z - adjacentZ;
                    if (Mathf.Abs(targetZDistance) < Mathf.Abs(adjacentZDistance))
                    {
                        targetLane = lane;
                    }
                    else
                    {
                        targetLane = adjacentLane;
                    }
                }
                else
                {
                    targetLane = lane;
                }
                entity.StartChangingLane(targetLane);
            }
        }
        public const float CHANGE_LANE_THRESOLD = 1;
    }
}