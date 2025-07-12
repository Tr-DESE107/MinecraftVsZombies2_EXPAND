using MVZ2.GameContent.Projectiles;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Properties;
using PVZEngine.Buffs;
using PVZEngine.Entities;
using PVZEngine.Level;
using Tools;
using UnityEngine;

namespace MVZ2.GameContent.Buffs.Level
{
    [BuffDefinition(VanillaBuffNames.Level.beaconMeteor)]
    public class BeaconMeteorBuff : BuffDefinition
    {
        public BeaconMeteorBuff(string nsp, string name) : base(nsp, name)
        {
        }
        public override void PostAdd(Buff buff)
        {
            base.PostAdd(buff);
            SetTimer(buff, new FrameTimer(30));
            SetState(buff, STATE_WAIT);
        }
        public override void PostUpdate(Buff buff)
        {
            base.PostUpdate(buff);
            var state = GetState(buff);
            switch (state)
            {
                case STATE_WAIT:
                    {
                        var timer = GetTimer(buff);
                        timer.Run();
                        if (timer.Expired)
                        {
                            timer.ResetTime(FALL_INTERVAL);
                            SetState(buff, STATE_FALL);
                        }
                    }
                    break;
                case STATE_FALL:
                    {
                        var timer = GetTimer(buff);
                        timer.Run();
                        if (timer.Expired)
                        {
                            timer.Reset();

                            SpawnMeteor(buff.Level, GetRNG(buff), GetFaction(buff), GetDamage(buff));

                            var count = GetCount(buff);
                            count--;
                            SetCount(buff, count);
                            if (count <= 0)
                            {
                                buff.Remove();
                            }
                        }
                    }
                    break;
            }
        }
        public static Entity SpawnMeteor(LevelEngine level, RandomGenerator rng, int faction, float damage)
        {
            var column = rng.Next(0, level.GetMaxColumnCount());
            var lane = rng.Next(0, level.GetMaxLaneCount());
            var x = level.GetEntityColumnX(column);
            var z = level.GetEntityLaneZ(lane);
            var y = level.GetGroundY(x, z);
            var landPos = new Vector3(x, y, z);

            var velX = rng.Next(VELOCITY_X_MIN, VELOCITY_X_MAX);
            var velZ = rng.Next(VELOCITY_X_MIN, VELOCITY_X_MAX);
            var velocity = new Vector3(velX, VELOCITY_Y, velZ);
            var pos = landPos - velocity * 30;

            var param = new SpawnParams();
            param.SetProperty(EngineEntityProps.FACTION, faction);
            param.SetProperty(VanillaEntityProps.DAMAGE, damage);
            var meteor = level.Spawn(VanillaProjectileID.beaconMeteor, pos, null, param);
            meteor.Velocity = velocity;
            meteor.PlaySound(VanillaSoundID.bombFalling);
            return meteor;
        }
        public static void SetDamage(Buff buff, float value) => buff.SetProperty(PROP_DAMAGE, value);
        public static float GetDamage(Buff buff) => buff.GetProperty<float>(PROP_DAMAGE);
        public static void SetCount(Buff buff, int value) => buff.SetProperty(PROP_COUNT, value);
        public static int GetCount(Buff buff) => buff.GetProperty<int>(PROP_COUNT);
        public static void SetFaction(Buff buff, int value) => buff.SetProperty(PROP_FACTION, value);
        public static int GetFaction(Buff buff) => buff.GetProperty<int>(PROP_FACTION);
        public static void SetState(Buff buff, int value) => buff.SetProperty(PROP_STATE, value);
        public static int GetState(Buff buff) => buff.GetProperty<int>(PROP_STATE);
        public static void SetTimer(Buff buff, FrameTimer value) => buff.SetProperty(PROP_TIMER, value);
        public static FrameTimer GetTimer(Buff buff) => buff.GetProperty<FrameTimer>(PROP_TIMER);
        public static void SetRNG(Buff buff, RandomGenerator value) => buff.SetProperty(PROP_RNG, value);
        public static RandomGenerator GetRNG(Buff buff) => buff.GetProperty<RandomGenerator>(PROP_RNG);
        public const float VELOCITY_X_MIN = -5;
        public const float VELOCITY_X_MAX = 5;
        public const float VELOCITY_Z_MIN = -5;
        public const float VELOCITY_Z_MAX = 5;
        public const float VELOCITY_Y = -25;
        public const int STATE_WAIT = 0;
        public const int STATE_FALL = 1;
        public const int FALL_INTERVAL = 6;
        public static readonly VanillaBuffPropertyMeta<float> PROP_DAMAGE = new VanillaBuffPropertyMeta<float>("damage");
        public static readonly VanillaBuffPropertyMeta<int> PROP_FACTION = new VanillaBuffPropertyMeta<int>("faction");
        public static readonly VanillaBuffPropertyMeta<int> PROP_COUNT = new VanillaBuffPropertyMeta<int>("count");
        public static readonly VanillaBuffPropertyMeta<FrameTimer> PROP_TIMER = new VanillaBuffPropertyMeta<FrameTimer>("timer");
        public static readonly VanillaBuffPropertyMeta<RandomGenerator> PROP_RNG = new VanillaBuffPropertyMeta<RandomGenerator>("rng");
        public static readonly VanillaBuffPropertyMeta<int> PROP_STATE = new VanillaBuffPropertyMeta<int>("state");
    }
}
