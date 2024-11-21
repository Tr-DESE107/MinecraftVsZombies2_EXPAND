using System.Collections.Generic;
using System.Linq;
using MVZ2.Vanilla;
using PVZEngine.Definitions;
using PVZEngine.Entities;
using PVZEngine.Grids;
using PVZEngine.Level;
using Tools;
using UnityEngine;

namespace MVZ2.GameContent.Areas
{
    [Definition(VanillaAreaNames.halloween)]
    public class Halloween : AreaDefinition
    {
        public Halloween(string nsp, string name) : base(nsp, name)
        {
            SetProperty(BuiltinAreaProps.DOOR_Z, 160f);
            SetProperty(EngineAreaProps.CART_REFERENCE, VanillaCartID.pumpkinCarriage);
            SetProperty(BuiltinLevelProps.MUSIC_ID, MusicID.halloween);
            SetProperty(BuiltinAreaProps.NIGHT_VALUE, 0.5f);
            for (int i = 0; i < 45; i++)
            {
                grids.Add(VanillaGridID.grass);
            }
        }
        public override void Setup(LevelEngine level)
        {
            base.Setup(level);
            level.SetProperty(PROP_RNG, level.CreateRNG());
        }
        public override void PrepareForBattle(LevelEngine level)
        {
            base.PrepareForBattle(level);
            SpawnStatues(level, level.GetStatueCount());
        }
        public override void PostHugeWaveEvent(LevelEngine level)
        {
            SpawnStatues(level, 2);
        }
        public override void PostFinalWaveEvent(LevelEngine level)
        {
            ReviveStatues(level);
        }
        public override float GetGroundY(float x, float z)
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
            List<LawnGrid> valid = new List<LawnGrid>();
            List<int> weights = new List<int>();
            for (int col = STATUE_MIN_COLUMN; col < level.GetMaxColumnCount(); col++)
            {
                for (int lane = 0; lane < level.GetMaxLaneCount(); lane++)
                {
                    var grid = level.GetGrid(col, lane);
                    if (grid.GetTakenEntities().Any(e => e != null && e.Type != EntityTypes.PLANT))
                        continue;
                    valid.Add(grid);
                    weights.Add(GetStatueWeight(col));
                }
            }
            count = Mathf.Clamp(count, 0, valid.Count);
            if (count <= 0)
                return;

            var rng = level.GetProperty<RandomGenerator>(PROP_RNG);
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

        public const string PROP_RNG = "HalloweenRNG";
        public const int STATUE_MIN_COLUMN = 5;
    }
}