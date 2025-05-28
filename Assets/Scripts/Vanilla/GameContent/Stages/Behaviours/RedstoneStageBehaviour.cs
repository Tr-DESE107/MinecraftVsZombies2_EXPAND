﻿using MVZ2.GameContent.Buffs.Enemies;
using MVZ2.Vanilla.Level;
using MVZ2.Vanilla.Properties;
using PVZEngine;
using PVZEngine.Definitions;
using PVZEngine.Entities;
using PVZEngine.Level;
using Tools;
using UnityEngine;

namespace MVZ2.GameContent.Stages
{
    public class RedstoneDropStageBehaviour : StageBehaviour
    {
        public RedstoneDropStageBehaviour(StageDefinition stageDef) : base(stageDef)
        {
        }
        public override void Start(LevelEngine level)
        {
            base.Start(level);
            SetRestoneRNG(level, level.CreateRNG());
            SetRedstoneChance(level, MIN_CHANCE);
            level.SetRedstoneCarrierChanceIncreament(CHANCE_INCREAMENT);
        }
        public override void PostWave(LevelEngine level, int wave)
        {
            base.PostWave(level, wave);
            var increament = level.GetRedstoneCarrierChanceIncreament();
            AddRedstoneChance(level, increament);
        }
        public override void PostEnemySpawned(Entity entity)
        {
            base.PostEnemySpawned(entity);
            var level = entity.Level;
            var chance = GetRedstoneChance(level);
            var rng = GetOrCreateRedstoneRNG(level);
            var value = rng.Next(100);
            if (value < chance)
            {
                entity.AddBuff<RedstoneCarrierBuff>();
                chance = Mathf.Max(MIN_CHANCE, chance + CHANCE_REDUCTION);
                SetRedstoneChance(level, chance);
            }
        }
        public static RandomGenerator GetOrCreateRedstoneRNG(LevelEngine level)
        {
            var rng = GetRedstoneRNG(level);
            if (rng == null)
            {
                rng = level.CreateRNG();
                SetRestoneRNG(level, rng);
            }
            return rng;
        }
        public static RandomGenerator GetRedstoneRNG(LevelEngine level)
        {
            return level.GetProperty<RandomGenerator>(PROP_REDSTONE_RNG);
        }
        public static void SetRestoneRNG(LevelEngine level, RandomGenerator value)
        {
            level.SetProperty(PROP_REDSTONE_RNG, value);
        }
        public static float GetRedstoneChance(LevelEngine level)
        {
            return level.GetProperty<float>(PROP_REDSTONE_CHANCE);
        }
        public static void SetRedstoneChance(LevelEngine level, float value)
        {
            level.SetProperty(PROP_REDSTONE_CHANCE, value);
        }
        public static void AddRedstoneChance(LevelEngine level, float value)
        {
            SetRedstoneChance(level, GetRedstoneChance(level) + value);
        }
        private const string PROP_REGION = "redstone_drop_stage";
        [LevelPropertyRegistry(PROP_REGION)]
        public static readonly VanillaLevelPropertyMeta<RandomGenerator> PROP_REDSTONE_RNG = new VanillaLevelPropertyMeta<RandomGenerator>("RedstoneRNG");
        [LevelPropertyRegistry(PROP_REGION)]
        public static readonly VanillaLevelPropertyMeta<float> PROP_REDSTONE_CHANCE = new VanillaLevelPropertyMeta<float>("RedstoneChance");
        public const int MIN_CHANCE = -15;
        public const int CHANCE_INCREAMENT = 10;
        public const int CHANCE_REDUCTION = -125;
    }
}
