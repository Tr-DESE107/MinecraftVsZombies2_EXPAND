using MVZ2.GameContent.Buffs.Enemies;
using MVZ2.GameContent.Difficulties;
using MVZ2.Vanilla.Level;
using MVZ2.Vanilla.Saves;
using MVZ2Logic;
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
        }
        public override void PostWave(LevelEngine level, int wave)
        {
            base.PostWave(level, wave);
            var increament = CHANCE_INCREAMENT;
            if (level.Difficulty == VanillaDifficulties.easy)
            {
                increament *= 2;
            }
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
            return level.GetProperty<RandomGenerator>(REDSTONE_RNG);
        }
        public static void SetRestoneRNG(LevelEngine level, RandomGenerator value)
        {
            level.SetProperty(REDSTONE_RNG, value);
        }
        public static int GetRedstoneChance(LevelEngine level)
        {
            return level.GetProperty<int>(REDSTONE_CHANCE);
        }
        public static void SetRedstoneChance(LevelEngine level, int value)
        {
            level.SetProperty(REDSTONE_CHANCE, value);
        }
        public static void AddRedstoneChance(LevelEngine level, int value)
        {
            SetRedstoneChance(level, GetRedstoneChance(level) + value);
        }

        public const string REDSTONE_RNG = "RedstoneRNG";
        public const string REDSTONE_CHANCE = "RedstoneChance";
        public const int MIN_CHANCE = -15;
        public const int CHANCE_INCREAMENT = 10;
        public const int CHANCE_REDUCTION = -125;
    }
}
