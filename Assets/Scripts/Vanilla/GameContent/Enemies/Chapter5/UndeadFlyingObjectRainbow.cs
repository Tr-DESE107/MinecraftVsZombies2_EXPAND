using System.Collections.Generic;
using MVZ2.GameContent.Effects;
using MVZ2.Vanilla.Entities;
using PVZEngine.Entities;
using PVZEngine.Grids;
using PVZEngine.Level;
using Tools;
using UnityEngine;

namespace MVZ2.GameContent.Enemies
{
    public class UFOBehaviourRainbow : UFOBehaviour
    {
        public UFOBehaviourRainbow() : base(UndeadFlyingObject.VARIANT_RAINBOW)
        {
        }
        public override void UpdateActionState(Entity entity, int state)
        {
            base.UpdateActionState(entity, state);
            switch (state)
            {
                case STATE_STAY:
                    UpdateStateStay(entity);
                    break;
            }
        }
        private void UpdateStateStay(Entity enemy)
        {
            EnterUpdate(enemy);
            var timer = GetOrInitStateTimer(enemy, STAY_TIME);
            timer.Run();
            if (timer.Expired)
            {
                SpawnRandomUFOs(enemy, 2);
                var effect = enemy.Spawn(VanillaEffectID.smokeCluster, enemy.GetCenter());
                effect.SetTint(new Color(1, 0.8f, 1, 1));
                enemy.Remove();
            }
        }
        public static void SpawnRandomUFOs(Entity rainbow, int count)
        {
            LevelEngine level = rainbow.Level;
            var rng = rainbow.RNG;

            HashSet<LawnGrid> possibleGrids = new HashSet<LawnGrid>();

            // 获取可以随机生成的UFO类型。
            List<int> typePool = new List<int>();
            UndeadFlyingObject.FillUFOVariantRandomPool(level, typePool);

            for (int i = 0; i < count; i++)
            {
                // 获取一个随机的UFO类型。
                var type = typePool.Random(rng);

                // 获取可以生成该UFO的网格。
                possibleGrids.Clear();
                UndeadFlyingObject.FillUFOPossibleSpawnGrids(level, type, rainbow.GetFaction(), possibleGrids);

                // 如果没有可用的网格，则跳过。
                if (possibleGrids.Count <= 0)
                    continue;

                // 检查冲突网格，确保不会与其他UFO冲突。
                var resultGrids = UndeadFlyingObject.FilterConflictSpawnGrids(level, possibleGrids);
                var targetGrid = resultGrids.Random(rng);
                SpawnRandomUFO(type, rainbow, targetGrid.Column, targetGrid.Lane);
            }
        }
        public static Entity SpawnRandomUFO(int type, Entity rainbow, int column, int lane)
        {
            var param = rainbow.GetSpawnParams();
            var ufo = rainbow.Spawn(VanillaEnemyID.ufo, rainbow.Position, param);
            ufo.Position = rainbow.Position;
            ufo.SetVariant(type);
            UndeadFlyingObject.SetTargetGridX(ufo, column);
            UndeadFlyingObject.SetTargetGridY(ufo, lane);
            return ufo;
        }

        public const int STAY_TIME = 150;
        public const int ACT_TIME = 30;
    }
}
