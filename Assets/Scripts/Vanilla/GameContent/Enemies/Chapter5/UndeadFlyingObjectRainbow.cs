using System.Collections.Generic;
using MVZ2.GameContent.Effects;
using MVZ2.Vanilla.Entities;
using PVZEngine;
using PVZEngine.Entities;
using PVZEngine.Grids;
using PVZEngine.Level;
using Tools;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

namespace MVZ2.GameContent.Enemies
{
    [EntityBehaviourDefinition(VanillaEnemyNames.undeadFlyingObjectRainbow)]
    public class UndeadFlyingObjectRainbow : UndeadFlyingObject
    {
        public UndeadFlyingObjectRainbow(string nsp, string name) : base(nsp, name)
        {
        }
        protected override void UpdateStateStay(Entity enemy)
        {
            base.UpdateStateStay(enemy);
            var timer = GetStateTimer(enemy);
            timer.Run();
            if (timer.Expired)
            {
                SpawnRandomUFOs(enemy, 2);
                var effect = enemy.Spawn(VanillaEffectID.smokeCluster, enemy.GetCenter());
                effect.SetTint(new Color(1, 0.8f, 1, 1));
                enemy.Remove();
            }
        }
        public override int GetStayTime() => STAY_TIME;
        public override int GetActTime() => ACT_TIME;
        public static void GetPossibleSpawnGrids(LevelEngine level, HashSet<LawnGrid> results)
        {
            var maxColumn = level.GetMaxColumnCount();
            var maxLane = level.GetMaxLaneCount();
            for (int x = 0; x < maxColumn; x++)
            {
                for (int y = 0; y < maxLane; y++)
                {
                    var grid = level.GetGrid(x, y);
                    if (grid != null)
                    {
                        results.Add(grid);
                    }
                }
            }
        }
        public static void SpawnRandomUFOs(Entity rainbow, int count)
        {
            LevelEngine level = rainbow.Level;
            var rng = rainbow.RNG;

            HashSet<LawnGrid> possibleGrids = new HashSet<LawnGrid>();

            // 获取可以随机生成的UFO类型。
            List<int> typePool = new List<int>();
            FillUFOTypeRandomPool(level, typePool);

            for (int i = 0; i < count; i++)
            {
                // 获取一个随机的UFO类型。
                var type = typePool.Random(rng);

                // 获取可以生成该UFO的网格。
                possibleGrids.Clear();
                FillUFOPossibleSpawnGrids(level, type, possibleGrids);

                // 如果没有可用的网格，则跳过。
                if (possibleGrids.Count <= 0)
                    continue;

                // 检查冲突网格，确保不会与其他UFO冲突。
                var resultGrids = FilterConflictSpawnGrids(level, possibleGrids);
                var targetGrid = resultGrids.Random(rng);
                var id = GetIDByType(type);
                SpawnRandomUFO(id, rainbow, targetGrid.Column, targetGrid.Lane);
            }
        }
        public static Entity SpawnRandomUFO(NamespaceID id, Entity rainbow, int column, int lane)
        {
            var param = rainbow.GetSpawnParams();
            var ufo = rainbow.Spawn(id, rainbow.Position, param);
            ufo.Position = rainbow.Position;
            SetTargetGridX(ufo, column);
            SetTargetGridY(ufo, lane);
            return ufo;
        }

        public const int STAY_TIME = 150;
        public const int ACT_TIME = 30;
    }
}
