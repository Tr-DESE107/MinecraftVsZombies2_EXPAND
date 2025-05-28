﻿using PVZEngine.Definitions;

namespace PVZEngine.Level
{
    [PropertyRegistryRegion(PropertyRegions.level)]
    public static class EngineLevelProps
    {
        public static readonly PropertyMeta<float> START_ENERGY = new PropertyMeta<float>("startEnergy");
        public static float GetStartEnergy(this LevelEngine level) => level.GetProperty<float>(START_ENERGY);
        public static void SetStartEnergy(this LevelEngine level, float value) => level.SetProperty(START_ENERGY, value);
        public static void SetStartEnergy(this StageDefinition stage, float value) => stage.SetProperty(START_ENERGY, value);

        public static readonly PropertyMeta<float> RECHARGE_SPEED = new PropertyMeta<float>("rechargeSpeed");
        public static float GetRechargeSpeed(this LevelEngine level) => level.GetProperty<float>(RECHARGE_SPEED);
        public static void SetRechargeSpeed(this LevelEngine level, float value) => level.SetProperty(RECHARGE_SPEED, value);
        public static void SetRechargeSpeed(this StageDefinition stage, float value) => stage.SetProperty(RECHARGE_SPEED, value);
    }
}
