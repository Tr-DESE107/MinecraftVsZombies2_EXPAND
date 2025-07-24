using MVZ2.GameContent.Buffs.Enemies;
using MVZ2.GameContent.Difficulties;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Properties;
using MVZ2.Vanilla.Saves;
using MVZ2Logic;
using PVZEngine;
using PVZEngine.Definitions;
using PVZEngine.Entities;
using PVZEngine.Level;
using Tools;
using UnityEngine;

namespace MVZ2.GameContent.Stages
{
    public class StarshardStageBehaviour : StageBehaviour
    {
        public StarshardStageBehaviour(StageDefinition stageDef) : base(stageDef)
        {
        }
        public override void Start(LevelEngine level)
        {
            base.Start(level);
            SetStarshardRNG(level, level.CreateRNG());
            SetStarshardChance(level, MIN_STARSHARD_CHANCE);
        }
        public override void PostWave(LevelEngine level, int wave)
        {
            base.PostWave(level, wave);
            var increament = level.GetStarshardCarrierChanceIncreament();
            AddStarshardChance(level, increament);
        }
        public override void PostEnemySpawned(Entity entity)
        {
            base.PostEnemySpawned(entity);
            if (!Global.Game.IsStarshardUnlocked())
                return;
            if (entity.HasNoReward())
                return;
            var level = entity.Level;
            var chance = GetStarshardChance(level);
            var rng = GetOrCreateStarshardRNG(level);
            var value = rng.Next(100);
            if (value < chance)
            {
                entity.AddBuff<StarshardCarrierBuff>();
                chance = Mathf.Max(MIN_STARSHARD_CHANCE, chance + STARSHARD_REDUCTION);
                SetStarshardChance(level, chance);
            }
        }
        public static RandomGenerator GetOrCreateStarshardRNG(LevelEngine level)
        {
            var rng = GetStarshardRNG(level);
            if (rng == null)
            {
                rng = level.CreateRNG();
                SetStarshardRNG(level, rng);
            }
            return rng;
        }
        public static RandomGenerator GetStarshardRNG(LevelEngine level)
        {
            return level.GetProperty<RandomGenerator>(PROP_STARSHARD_RNG);
        }
        public static void SetStarshardRNG(LevelEngine level, RandomGenerator value)
        {
            level.SetProperty(PROP_STARSHARD_RNG, value);
        }
        public static float GetStarshardChance(LevelEngine level)
        {
            return level.GetProperty<float>(PROP_STARSHARD_CHANCE);
        }
        public static void SetStarshardChance(LevelEngine level, float value)
        {
            level.SetProperty(PROP_STARSHARD_CHANCE, value);
        }
        public static void AddStarshardChance(LevelEngine level, float value)
        {
            SetStarshardChance(level, GetStarshardChance(level) + value);
        }

        private const string PROP_REGION = "starshard_drop_stage";
        [LevelPropertyRegistry(PROP_REGION)]
        public static readonly VanillaLevelPropertyMeta<RandomGenerator> PROP_STARSHARD_RNG = new VanillaLevelPropertyMeta<RandomGenerator>("StarshardRNG");
        [LevelPropertyRegistry(PROP_REGION)]
        public static readonly VanillaLevelPropertyMeta<float> PROP_STARSHARD_CHANCE = new VanillaLevelPropertyMeta<float>("StarshardChance");
        public const int MIN_STARSHARD_CHANCE = -15;
        public const int STARSHARD_INCREAMENT = 10;
        public const int STARSHARD_REDUCTION = -125;
    }
}
