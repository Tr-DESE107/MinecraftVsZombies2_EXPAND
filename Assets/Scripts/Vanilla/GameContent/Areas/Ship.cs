using MVZ2.Vanilla.Level;
using MVZ2.Vanilla.Properties;
using MVZ2Logic.Level;
using PVZEngine.Definitions;
using PVZEngine.Level;
using PVZEngine.Models;
using UnityEngine;

namespace MVZ2.GameContent.Areas
{
    [AreaDefinition(VanillaAreaNames.ship)]
    public class Ship : AreaDefinition
    {
        public Ship(string nsp, string name) : base(nsp, name)
        {
        }
        public override void Setup(LevelEngine level)
        {
            base.Setup(level);
            SetSkyOffsetSpeed(level, SKY_OFFSET_SPEED_NORMAL);
        }
        public override void Update(LevelEngine level)
        {
            base.Update(level);
            var skyOffsetSpeed = GetSkyOffsetSpeed(level);
            var targetSpeed = SKY_OFFSET_SPEED_NORMAL;
            if (level.IsDuringHugeWave())
            {
                targetSpeed = SKY_OFFSET_SPEED_FAST;
            }
            var accel = (targetSpeed - skyOffsetSpeed) * SKY_OFFSET_ACCELERATION;
            if (skyOffsetSpeed != targetSpeed)
            {
                if (skyOffsetSpeed < targetSpeed == skyOffsetSpeed + accel > targetSpeed)
                {
                    skyOffsetSpeed = targetSpeed;
                }
                else
                {
                    skyOffsetSpeed += accel;
                }
            }
            SetSkyOffsetSpeed(level, skyOffsetSpeed);
            level.SetModelAnimatorFloat("SkyOffsetSpeed", skyOffsetSpeed);
        }
        public static float GetSkyOffsetSpeed(LevelEngine level) => level.GetProperty<float>(PROP_SKY_OFFSET_SPEED);
        public static void SetSkyOffsetSpeed(LevelEngine level, float value) => level.SetProperty<float>(PROP_SKY_OFFSET_SPEED, value);

        public const float SKY_OFFSET_SPEED_NORMAL = 1;
        public const float SKY_OFFSET_SPEED_FAST = 10;
        public const float SKY_OFFSET_ACCELERATION = 0.1f;
        public static readonly VanillaLevelPropertyMeta<float> PROP_SKY_OFFSET_SPEED = new VanillaLevelPropertyMeta<float>("sky_offset_speed");
    }
}