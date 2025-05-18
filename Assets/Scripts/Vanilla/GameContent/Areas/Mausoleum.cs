using System.Collections.Generic;
using System.Linq;
using MVZ2.GameContent.Effects;
using MVZ2.GameContent.Enemies;
using MVZ2.GameContent.Obstacles;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Level;
using MVZ2.Vanilla.Properties;
using MVZ2Logic.Level;
using PVZEngine;
using PVZEngine.Definitions;
using PVZEngine.Entities;
using PVZEngine.Grids;
using PVZEngine.Level;
using Tools;
using UnityEngine;

namespace MVZ2.GameContent.Areas
{
    [AreaDefinition(VanillaAreaNames.mausoleum)]
    public class Mausoleum : AreaDefinition
    {
        public Mausoleum(string nsp, string name) : base(nsp, name)
        {
        }
        public override void Setup(LevelEngine level)
        {
            base.Setup(level);
            SetRNG(level, level.CreateRNG());
        }
        public override void PrepareForBattle(LevelEngine level)
        {
            base.PrepareForBattle(level);
            SpawnSpawners(level, level.GetSpawnerCount());
        }
        public override void PostHugeWaveEvent(LevelEngine level)
        {
            base.PostHugeWaveEvent(level);
            if (level.GetSpawnerCount() <= 0)
                return;
            SpawnSpawners(level, 2);
        }
        public void SpawnSpawners(LevelEngine level, int count)
        {
            if (count <= 0)
                return;
            var statueDef = level.Content.GetEntityDefinition(VanillaObstacleID.monsterSpawner);
            if (statueDef == null)
                return;
            List<LawnGrid> valid = new List<LawnGrid>();
            List<int> weights = new List<int>();

            var layersToTake = statueDef.GetGridLayersToTake();
            for (int col = SPAWNER_MIN_COLUMN; col < level.GetMaxColumnCount(); col++)
            {
                for (int lane = 0; lane < level.GetMaxLaneCount(); lane++)
                {
                    var grid = level.GetGrid(col, lane);
                    if (layersToTake.Any(l => { var e = grid.GetLayerEntity(l); return e != null && e.Type != EntityTypes.PLANT; }))
                        continue;
                    valid.Add(grid);
                    weights.Add(GetSpawnerWeight(col));
                }
            }
            count = Mathf.Clamp(count, 0, valid.Count);
            if (count <= 0)
                return;

            var rng = GetRNG(level);
            var grids = valid.WeightedRandomTake(weights.ToArray(), count, rng);
            foreach (var grid in grids)
            {
                var pos = grid.GetEntityPosition();
                var spawner = level.Spawn(VanillaObstacleID.monsterSpawner, pos, null);
                var entityToSpawn = GetEntityToSpawn(rng);
                MonsterSpawner.SetEntityToSpawn(spawner, entityToSpawn);

                var param = spawner.GetSpawnParams();
                param.SetProperty(VanillaEntityProps.UPDATE_BEFORE_GAME, true);
                var embers = spawner.Spawn(VanillaEffectID.spawnerAppearEmbers, spawner.GetCenter(), param);
                spawner.PlaySound(VanillaSoundID.odd);
            }
        }
        private NamespaceID GetEntityToSpawn(RandomGenerator rng)
        {
            return entitiesToSpawn.Random(rng);
        }
        private int GetSpawnerWeight(int column)
        {
            return column - SPAWNER_MIN_COLUMN + 1;
        }
        public static RandomGenerator GetRNG(LevelEngine level) => level.GetBehaviourField<RandomGenerator>(PROP_RNG);
        public static void SetRNG(LevelEngine level, RandomGenerator rng) => level.SetBehaviourField(PROP_RNG, rng);
        public static readonly VanillaLevelPropertyMeta PROP_RNG = new VanillaLevelPropertyMeta("SpawnerRNG");
        public static readonly NamespaceID[] entitiesToSpawn = new NamespaceID[]
        {
            VanillaEnemyID.zombie,
            VanillaEnemyID.leatherCappedZombie,
            VanillaEnemyID.ironHelmettedZombie
        };
        public const int SPAWNER_MIN_COLUMN = 5;
    }
}