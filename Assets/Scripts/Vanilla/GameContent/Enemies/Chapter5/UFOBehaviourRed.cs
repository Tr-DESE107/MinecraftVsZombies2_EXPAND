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
    public class UFOBehaviourRed : UFOBehaviour
    {
        public UFOBehaviourRed() : base(UndeadFlyingObject.VARIANT_RED)
        {
        }
        public override void GetPossibleSpawnGrids(LevelEngine level, HashSet<LawnGrid> results)
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
        public override void UpdateActionState(Entity entity, int state)
        {
            base.UpdateActionState(entity, state);
            switch (state)
            {
                case STATE_STAY:
                    UpdateStateStay(entity);
                    break;
                case STATE_ACT:
                    UpdateStateAct(entity);
                    break;
                case STATE_LEAVE:
                    UpdateStateLeave(entity);
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
                SetUFOState(enemy, STATE_ACT);
                timer.ResetTime(ACT_TIME);
            }
        }
        private void UpdateStateAct(Entity enemy)
        {
            LeaveUpdate(enemy);

            var timer = GetOrInitStateTimer(enemy, ACT_TIME);
            timer.Run();
            if (timer.PassedFrameFromMax(RELEASE_ZOMBIE_TIME))
            {
                var enemyID = enemyPool.Random(enemy.RNG);
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
        private void UpdateStateLeave(Entity enemy)
        {
            LeaveUpdate(enemy);
        }
        public const int STAY_TIME = 90;
        public const int ACT_TIME = 60;
        public const int RELEASE_ZOMBIE_TIME = 30;
        public static NamespaceID[] enemyPool = new NamespaceID[]
        {
            VanillaEnemyID.zombie,
            VanillaEnemyID.leatherCappedZombie,
            VanillaEnemyID.ironHelmettedZombie,
        };
    }
}
