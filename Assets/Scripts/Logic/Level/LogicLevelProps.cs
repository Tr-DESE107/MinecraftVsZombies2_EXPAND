﻿using PVZEngine;
using PVZEngine.Level;
using Tools;
using UnityEngine;

namespace MVZ2Logic.Level
{
    [PropertyRegistryRegion(PropertyRegions.level)]
    public static class LogicLevelProps
    {
        public static readonly PropertyMeta<Color> SCREEN_COVER = new PropertyMeta<Color>("screenCover");
        public static Color GetScreenCover(this LevelEngine level)
        {
            return level.GetProperty<Color>(SCREEN_COVER);
        }
        public static void SetScreenCover(this LevelEngine level, Color value)
        {
            level.SetProperty(SCREEN_COVER, value);
        }
        public static readonly PropertyMeta<bool> PAUSE_DISABLED = new PropertyMeta<bool>("pause_disabled");
        public static bool IsPauseDisabled(this LevelEngine level)
        {
            return level.GetProperty<bool>(PAUSE_DISABLED);
        }
        public static void SetPauseDisabled(this LevelEngine level, bool value)
        {
            level.SetProperty(PAUSE_DISABLED, value);
        }
        public static readonly PropertyMeta<float> CAMERA_ROTATION = new PropertyMeta<float>("cameraRotation");
        public static float GetCameraRotation(this LevelEngine level)
        {
            return level.GetProperty<float>(CAMERA_ROTATION);
        }
        public static void SetCameraRotation(this LevelEngine level, float value)
        {
            level.SetProperty(CAMERA_ROTATION, value);
        }

        public static readonly PropertyMeta<bool> MUSIC_LOW_QUALITY = new PropertyMeta<bool>("musicLowQuality");
        public static bool IsMusicLowQuality(this LevelEngine level)
        {
            return level.GetProperty<bool>(MUSIC_LOW_QUALITY);
        }

        public static readonly PropertyMeta<bool> GRAPHICS_DOWNGRADE = new PropertyMeta<bool>("graphicsDowngrade");
        public static bool AreGraphicsDowngrade(this LevelEngine level)
        {
            return level.GetProperty<bool>(GRAPHICS_DOWNGRADE);
        }

        #region RNG
        public static readonly PropertyMeta<RandomGenerator> ARTIFACT_RNG = new PropertyMeta<RandomGenerator>("artifactRNG");
        public static RandomGenerator GetArtifactRNG(this LevelEngine level)
        {
            var rng = level.GetProperty<RandomGenerator>(ARTIFACT_RNG);
            if (rng == null)
            {
                rng = level.CreateRNG();
                level.SetArtifactRNG(rng);
            }
            return rng;
        }
        public static void SetArtifactRNG(this LevelEngine level, RandomGenerator value)
        {
            level.SetProperty(ARTIFACT_RNG, value);
        }
        #endregion
    }
}
