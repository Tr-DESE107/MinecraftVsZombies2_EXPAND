using MVZ2.GameContent.Bosses;
using MVZ2.GameContent.Buffs.Level;
using MVZ2.GameContent.Pickups;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Level;
using MVZ2Logic.Level;
using PVZEngine.Definitions;
using PVZEngine.Entities;
using PVZEngine.Level;
using UnityEngine;
using MVZ2.GameContent.Bosses;
using MVZ2.GameContent.Buffs.Enemies;
using MVZ2.GameContent.Buffs.Level;
using MVZ2.GameContent.Contraptions;
using MVZ2.GameContent.Enemies;
using MVZ2.GameContent.ProgressBars;
using MVZ2.GameContent.Talk;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Callbacks;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Level;
using MVZ2.Vanilla.Properties;
using MVZ2Logic.Level;
using PVZEngine;
using PVZEngine.Callbacks;
using PVZEngine.Definitions;
using PVZEngine.Entities;
using PVZEngine.Level;
using UnityEngine;

namespace MVZ2.GameContent.Stages
{
    public class Castle15StageBehaviour : BossStageBehaviour
    {
        public Castle15StageBehaviour(StageDefinition stageDef) : base(stageDef)
        {
            
        }
        protected override void AfterFinalWaveUpdate(LevelEngine level)
        {
            base.AfterFinalWaveUpdate(level);
            WitherTransitionUpdate(level);
        }

        bool SeijaSpawn = false;

        private void WitherTransitionUpdate(LevelEngine level)
        {
            if (!level.HasBuff<WitherTransitionBuff>())
            {
                level.AddBuff<WitherTransitionBuff>();
            }

            
            if (SeijaSpawn == false)
            {
                var x = VanillaLevelExt.ENEMY_RIGHT_BORDER;
                var z = level.GetEntityLaneZ(level.GetMaxLaneCount() / 2);
                var y = level.GetGroundY(x, z);
                var seija = level.Spawn(VanillaBossID.seija, new Vector3(x, y, z), null);
                Seija.StartState(seija, VanillaEntityStates.SEIJA_FRONTFLIP);
                SeijaSpawn = true;
            }
        }
        protected override void BossFightWaveUpdate(LevelEngine level)
        {
            base.BossFightWaveUpdate(level);
            WitherFightUpdate(level);
        }

        private void WitherFightUpdate(LevelEngine level)
        {
            // 凋灵战斗
            // 如果不存在Boss，或者所有Boss死亡，进入BOSS后阶段。
            // 如果有Boss存活，不停生成怪物。
            if (!level.EntityExists(e => e.Type == EntityTypes.BOSS && e.IsHostileEntity() && !e.IsDead))
            {
                level.WaveState = VanillaLevelStates.STATE_AFTER_BOSS;
                level.StopMusic();
                
                    Vector3 position;
                    
                        var x = level.GetLawnCenterX();
                        var z = level.GetLawnCenterZ();
                        var y = level.GetGroundY(x, z);
                        position = new Vector3(x, y, z);
                    
                    level.Produce(VanillaPickupID.clearPickup, position, null);
                
            }
            else
            {
                RunBossWave(level);
            }
        }
        protected override void AfterBossWaveUpdate(LevelEngine level)
        {
            base.AfterBossWaveUpdate(level);
            ClearEnemies(level);

            if (!level.IsRerun)
            {
                if (!level.IsCleared && !level.EntityExists(e => e.Type == EntityTypes.BOSS && e.IsHostileEntity()))
                {
                    if (!level.HasBuff<WitherClearedBuff>())
                    {
                        level.AddBuff<WitherClearedBuff>();
                    }
                }
            }
        }
    }
}
