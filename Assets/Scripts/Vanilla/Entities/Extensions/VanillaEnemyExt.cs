﻿using System;
using MVZ2.GameContent.Buffs.Enemies;
using MVZ2.GameContent.Effects;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Callbacks;
using PVZEngine.Buffs;
using PVZEngine.Callbacks;
using PVZEngine.Entities;
using UnityEngine;

namespace MVZ2.Vanilla.Entities
{
    public static class VanillaEnemyExt
    {
        public static void Neutralize(this Entity enemy)
        {
            enemy.DropRewards();
            if (enemy.IsNeutralized())
                return;

            var result = new CallbackResult(true);
            enemy.Level.Triggers.RunCallbackWithResult(VanillaLevelCallbacks.PRE_ENEMY_NEUTRALIZE, new EntityCallbackParams(enemy), result);
            if (!result.GetValue<bool>())
                return;
            enemy.SetNeutralized(true);
            enemy.Level.Triggers.RunCallback(VanillaLevelCallbacks.POST_ENEMY_NEUTRALIZE, new EntityCallbackParams(enemy));
        }
        public static void DropRewards(this Entity enemy)
        {
            if (enemy.HasNoReward())
                return;
            enemy.Level.Triggers.RunCallback(VanillaLevelCallbacks.ENEMY_DROP_REWARDS, new EntityCallbackParams(enemy));
        }
        public static void InflictWeakness(this Entity enemy, int time)
        {
            Buff buff = enemy.GetFirstBuff<EnemyWeaknessBuff>();
            if (buff == null)
            {
                buff = enemy.AddBuff<EnemyWeaknessBuff>();
            }
            buff.SetProperty(EnemyWeaknessBuff.PROP_TIMEOUT, time);
        }

        public static void InflictRegenerationBuff(this Entity enemy, float Heal, int time)
        {
            Buff buff = enemy.GetFirstBuff<RegenerationBuff>();
            if (buff == null)
            {
                buff = enemy.AddBuff<RegenerationBuff>();
            }
            buff.SetProperty(RegenerationBuff.PROP_HEAL_AMOUNT, Heal);
            buff.SetProperty(RegenerationBuff.PROP_TIMEOUT, time);
        }

        public static void InflictCorropoisonBuff(this Entity enemy, float damage, int time)
        {
            Buff buff = enemy.GetFirstBuff<CorropoisonBuff>();
            if (buff == null)
            {
                buff = enemy.AddBuff<CorropoisonBuff>();
            }
            buff.SetProperty(CorropoisonBuff.PROP_DAMAGE_AMOUNT, damage);
            buff.SetProperty(CorropoisonBuff.PROP_TIMEOUT, time);
        }

        public static void UpdateWalkVelocity(this Entity enemy)
        {
            var velocity = enemy.Velocity;
            var speed = enemy.GetSpeed() * WALK_SPEED_FACTOR;
            if (Mathf.Abs(velocity.x) < speed)
            {
                float min = Mathf.Min(speed, -speed);
                float max = Mathf.Max(speed, -speed);
                float direciton = enemy.GetFacingX();
                velocity.x += speed * direciton;
                velocity.x = Mathf.Clamp(velocity.x, min, max);
            }
            enemy.Velocity = velocity;
        }
        public const float WALK_SPEED_FACTOR = 0.4f; // 怪物的移速乘算倍率，默认0.4倍
        public static void FaintRemove(this Entity enemy)
        {
            var callbackResult = new CallbackResult(true);
            enemy.Level.Triggers.RunCallbackWithResultFiltered(VanillaLevelCallbacks.PRE_ENEMY_FAINT, new EntityCallbackParams(enemy), callbackResult, enemy.GetDefinitionID());
            if (callbackResult.GetValue<bool>())
            {
                var param = enemy.GetSpawnParams();
                param.SetProperty(EngineEntityProps.SIZE, enemy.GetSize());
                enemy.Level.Spawn(VanillaEffectID.smoke, enemy.Position, enemy, param);
                enemy.Remove();
                enemy.Level.Triggers.RunCallbackFiltered(VanillaLevelCallbacks.POST_ENEMY_FAINT, new EntityCallbackParams(enemy), enemy.GetDefinitionID());
            }
        }

        #region 骑乘
        public static void RideOn(this Entity passenger, Entity horse)
        {
            if (passenger == null || horse == null)
                return;
            passenger.GetOffPassenger();
            horse.GetOffHorse();

            var passengerBuff = passenger.AddBuff<RidingPassengerBuff>();
            RidingPassengerBuff.SetRidingEntity(passengerBuff, horse);
            passenger.UpdatePassengerPosition(horse);

            var horseBuff = horse.AddBuff<BeingRidenBuff>();
            BeingRidenBuff.SetPassenger(horseBuff, passenger);
        }
        public static void GetOffPassenger(this Entity passenger)
        {
            if (passenger == null)
                return;
            var horse = passenger.GetRidingEntity();
            if (horse != null)
            {
                horse.RemoveBuffs<BeingRidenBuff>();
            }
            passenger.RemoveBuffs<RidingPassengerBuff>();
        }
        public static void GetOffHorse(this Entity horse)
        {
            if (horse == null)
                return;
            var passenger = horse.GetRideablePassenger();
            if (passenger != null)
            {
                passenger.RemoveBuffs<RidingPassengerBuff>();
            }
            horse.RemoveBuffs<BeingRidenBuff>();
        }
        public static Entity GetRidingEntity(this Entity entity)
        {
            var buff = entity.GetFirstBuff<RidingPassengerBuff>();
            if (buff == null)
                return null;
            return RidingPassengerBuff.GetRidingEntity(buff);
        }
        public static Entity GetRideablePassenger(this Entity entity)
        {
            var buff = entity.GetFirstBuff<BeingRidenBuff>();
            if (buff == null)
                return null;
            return BeingRidenBuff.GetPassenger(buff);
        }
        public static void UpdatePassengerPosition(this Entity passenger, Entity horse)
        {
            passenger.Position = horse.Position + horse.GetPassengerOffset();
        }
        #endregion

        public static void CheckAlignWithLane(this Entity entity)
        {
            if (entity.IsChangingLane())
                return;
            var level = entity.Level;

            var minLane = 0;
            var maxLane = level.GetMaxLaneCount() - 1;

            var lane = Mathf.Clamp(entity.GetLane(), minLane, maxLane);
            var targetZ = level.GetEntityLaneZ(lane);
            var targetZDistance = entity.Position.z - targetZ;

            if (Mathf.Abs(targetZDistance) < CHANGE_LANE_THRESOLD)
                return;

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
        public const float CHANGE_LANE_THRESOLD = 1;
        public static void Unfreeze(this Entity entity)
        {
            entity.RemoveBuffs<SlowBuff>();
        }
        public static void Slow(this Entity entity, int time)
        {
            var buff = entity.GetFirstBuff<SlowBuff>();
            if (buff == null)
            {
                entity.PlaySound(VanillaSoundID.freeze);
                buff = entity.AddBuff<SlowBuff>();
            }
            SlowBuff.SetTimeout(buff, time);
        }
    }
}
