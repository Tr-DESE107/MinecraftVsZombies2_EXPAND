using MVZ2.GameContent.Buffs.Enemies;
using MVZ2.Vanilla.Callbacks;
using PVZEngine.Buffs;
using PVZEngine.Entities;
using PVZEngine.Triggers;
using UnityEngine;

namespace MVZ2.Vanilla.Entities
{
    public static class VanillaEnemyExt
    {
        public static void Neutralize(this Entity enemy)
        {
            if (enemy.IsNeutralized())
                return;

            var result = new TriggerResultBoolean();
            result.Result = true;
            enemy.Level.Triggers.RunCallback(VanillaLevelCallbacks.PRE_ENEMY_NEUTRALIZE, result, c => c(enemy, result));
            if (result.Result == false)
                return;
            enemy.SetNeutralized(true);
            enemy.DropRewards();
            enemy.Level.Triggers.RunCallback(VanillaLevelCallbacks.POST_ENEMY_NEUTRALIZE, c => c(enemy));
        }
        public static void DropRewards(this Entity enemy)
        {
            if (enemy.HasNoReward())
                return;
            enemy.Level.Triggers.RunCallback(VanillaLevelCallbacks.ENEMY_DROP_REWARDS, c => c(enemy));
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
        public static void UpdateWalkVelocity(this Entity enemy)
        {
            var velocity = enemy.Velocity;
            var speed = enemy.GetSpeed() * 0.4f;
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

        #region 骑乘
        public static void RideOn(this Entity passenger, Entity horse)
        {
            if (passenger == null || horse == null)
                return;
            passenger.GetOffPassenger();
            horse.GetOffHorse();

            var passengerBuff = passenger.AddBuff<RidingPassengerBuff>();
            RidingPassengerBuff.SetRidingEntity(passengerBuff, horse);

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
        #endregion
    }
}
