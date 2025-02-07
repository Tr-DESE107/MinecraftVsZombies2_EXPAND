using System.Collections.Generic;
using System.Linq;
using MVZ2.GameContent.Effects;
using MVZ2.GameContent.Enemies;
using MVZ2.GameContent.Obstacles;
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
    [AreaDefinition(VanillaAreaNames.halloween)]
    public class Halloween : AreaDefinition
    {
        public Halloween(string nsp, string name) : base(nsp, name)
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
            SpawnStatues(level, level.GetStatueCount());
        }
        public override void PostHugeWaveEvent(LevelEngine level)
        {
            base.PostHugeWaveEvent(level);
            if (level.GetStatueCount() <= 0)
                return;
            SpawnStatues(level, 2);
        }
        public override void PostFinalWaveEvent(LevelEngine level)
        {
            ReviveStatues(level);
        }
        public override float GetGroundY(LevelEngine level, float x, float z)
        {
            switch (x)
            {
                case > 185:
                    return 0;
                case > 175:
                    // 地面和第一层的交界处
                    return Mathf.Lerp(0, 48, (185 - x) / 10);
                case > 140:
                    return 48;
                case > 130:
                    // 第一层和第二层的交界处
                    return Mathf.Lerp(48, 96, (140 - x) / 10);
                case > 95:
                    return 96;
                case > 85:
                    // 第二层和第三层的交界处
                    return Mathf.Lerp(96, 144, (95 - x) / 10);
                default:
                    return 144;
            }
        }
        public void SpawnStatues(LevelEngine level, int count)
        {
            if (count <= 0)
                return;
            var statueDef = level.Content.GetEntityDefinition(VanillaObstacleID.gargoyleStatue);
            if (statueDef == null)
                return;
            List<LawnGrid> valid = new List<LawnGrid>();
            List<int> weights = new List<int>();

            var layersToTake = statueDef.GetGridLayersToTake();
            for (int col = STATUE_MIN_COLUMN; col < level.GetMaxColumnCount(); col++)
            {
                for (int lane = 0; lane < level.GetMaxLaneCount(); lane++)
                {
                    var grid = level.GetGrid(col, lane);
                    if (layersToTake.Any(l => { var e = grid.GetLayerEntity(l); return e != null && e.Type != EntityTypes.PLANT; }))
                        continue;
                    valid.Add(grid);
                    weights.Add(GetStatueWeight(col));
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
                level.Spawn(VanillaObstacleID.gargoyleStatue, pos, null);
            }
        }
        public void ReviveStatues(LevelEngine level)
        {
            foreach (var statue in level.FindEntities(VanillaObstacleID.gargoyleStatue))
            {
                var gargoyle = level.Spawn(VanillaEnemyID.gargoyle, statue.Position, statue);
                gargoyle.Health = gargoyle.GetMaxHealth() * statue.Health / statue.GetMaxHealth();
                level.Spawn(VanillaEffectID.thunderBolt, statue.Position, statue);
                statue.Die();
            }
        }

        private int GetStatueWeight(int column)
        {
            return column - STATUE_MIN_COLUMN + 1;
        }
        public static RandomGenerator GetRNG(LevelEngine level) => level.GetBehaviourField<RandomGenerator>(ID, PROP_RNG);
        public static void SetRNG(LevelEngine level, RandomGenerator rng) => level.SetBehaviourField(ID, PROP_RNG, rng);

        private static readonly NamespaceID ID = VanillaAreaID.halloween;
        public static readonly VanillaLevelPropertyMeta PROP_RNG = new VanillaLevelPropertyMeta("HalloweenRNG");
        public const int STATUE_MIN_COLUMN = 5;
    }
}