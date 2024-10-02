using MVZ2.GameContent;
using MVZ2.Vanilla.Buffs;
using PVZEngine.Definitions;
using PVZEngine.Level;
using Tools;
using UnityEngine;

namespace MVZ2.Vanilla
{
    public class StarshardStageBehaviour : StageBehaviour
    {
        public StarshardStageBehaviour(StageDefinition stageDef) : base(stageDef)
        {
        }
        public override void Start(LevelEngine level)
        {
            base.Start(level);
            level.SetStarshardRNG(level.CreateRNG());
            level.SetStarshardChance(MIN_STARSHARD_CHANCE);
        }
        public override void PostWave(LevelEngine level, int wave)
        {
            base.PostWave(level, wave);
            level.AddStarshardChance(STARSHARD_INCREAMENT);
        }
        public override void PostEnemySpawned(Entity entity)
        {
            base.PostEnemySpawned(entity);
            var level = entity.Level;
            var chance = level.GetStarshardChance();
            var rng = GetOrCreateStarshardRNG(level);
            var value = rng.Next(100);
            if (value < chance)
            {
                entity.AddBuff<StarshardCarrierBuff>();
                chance = Mathf.Max(MIN_STARSHARD_CHANCE, chance + STARSHARD_REDUCTION);
                level.SetStarshardChance(chance);
            }
        }
        public static RandomGenerator GetOrCreateStarshardRNG(LevelEngine level)
        {
            var rng = level.GetStarshardRNG();
            if (rng == null)
            {
                rng = level.CreateRNG();
                level.SetStarshardRNG(rng);
            }
            return rng;
        }
        public const int MIN_STARSHARD_CHANCE = -25;
        public const int STARSHARD_INCREAMENT = 10;
        public const int STARSHARD_REDUCTION = -125;
    }
}
