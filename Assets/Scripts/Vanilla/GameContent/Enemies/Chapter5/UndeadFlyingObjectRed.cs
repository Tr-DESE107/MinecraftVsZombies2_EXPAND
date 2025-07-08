using System.Collections.Generic;
using MVZ2.GameContent.Buffs;
using MVZ2.GameContent.Buffs.Contraptions;
using MVZ2.Vanilla.Entities;
using PVZEngine;
using PVZEngine.Entities;
using PVZEngine.Grids;
using PVZEngine.Level;
using Tools;

namespace MVZ2.GameContent.Enemies
{
    [EntityBehaviourDefinition(VanillaEnemyNames.undeadFlyingObjectRed)]
    public class UndeadFlyingObjectRed : UndeadFlyingObject
    {
        public UndeadFlyingObjectRed(string nsp, string name) : base(nsp, name)
        {
        }
        protected override void UpdateStateStay(Entity enemy)
        {
            base.UpdateStateStay(enemy);
            var timer = GetStateTimer(enemy);
            timer.Run();
            if (timer.Expired)
            {
                SetUFOState(enemy, STATE_ACT);
                timer.ResetTime(GetActTime());
            }
        }
        protected override void UpdateStateAct(Entity enemy)
        {
            base.UpdateStateAct(enemy);
            var timer = GetStateTimer(enemy);
            timer.Run();
            if (timer.PassedFrameFromMax(RED_RELEASE_ZOMBIE_TIME))
            {
                var enemyID = redEnemyPool.Random(enemy.RNG);
                var param = enemy.GetSpawnParams();
                param.OnApply += (e) =>
                {
                    e.AddBuff<SummonedByUFOBuff>();
                    WhiteFlashBuff.AddToEntity(e, 30);
                };
                enemy.Spawn(enemyID, enemy.Position, param);
            }
            if (timer.Expired)
            {
                SetUFOState(enemy, STATE_LEAVE);
            }
        }
        public override int GetStayTime() => STAY_TIME;
        public override int GetActTime() => ACT_TIME;

        public static bool CanSpawn(LevelEngine level)
        {
            return true;
        }
        public static void GetPossibleSpawnGrids(LevelEngine level, HashSet<LawnGrid> results)
        {
            var maxColumn = level.GetMaxColumnCount();
            var maxLane = level.GetMaxLaneCount();
            for (int x = maxColumn - 4; x < maxColumn; x++)
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

        public const int STAY_TIME = 90;
        public const int ACT_TIME = 60;

        public const int RED_RELEASE_ZOMBIE_TIME = 30;
        public static NamespaceID[] redEnemyPool = new NamespaceID[]
        {
            VanillaEnemyID.zombie,
            VanillaEnemyID.leatherCappedZombie,
            VanillaEnemyID.ironHelmettedZombie,
        };
    }
}
