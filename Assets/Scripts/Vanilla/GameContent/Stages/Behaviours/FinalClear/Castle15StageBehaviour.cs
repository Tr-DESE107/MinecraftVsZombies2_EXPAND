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

        public const string PROP_REGION = "castle15_stage_behaviour";
        [LevelPropertyRegistry(PROP_REGION)]
        public static readonly VanillaLevelPropertyMeta<bool> FIELD_SEIJA_SPAWNED =
            new VanillaLevelPropertyMeta<bool>("SeijaSpawned");

        private void WitherTransitionUpdate(LevelEngine level)
        {
            if (!level.HasBuff<WitherTransitionBuff>())
            {
                level.AddBuff<WitherTransitionBuff>();
            }


            if (!level.GetProperty<bool>(FIELD_SEIJA_SPAWNED))
            {
                var x = VanillaLevelExt.ENEMY_RIGHT_BORDER;
                var z = level.GetEntityLaneZ(level.GetMaxLaneCount() / 2);
                var y = level.GetGroundY(x, z);
                var seija = level.Spawn(VanillaBossID.seija, new Vector3(x, y, z), null);
                seija.InflictRegenerationBuff(1000f, 300);
                Seija.StartState(seija, VanillaEntityStates.SEIJA_FRONTFLIP);
                level.SetProperty(FIELD_SEIJA_SPAWNED, true);
            }
        }
        protected override void BossFightWaveUpdate(LevelEngine level)
        {
            base.BossFightWaveUpdate(level);
            WitherFightUpdate(level);
        }

        private void WitherFightUpdate(LevelEngine level)
        {
            // 检查特定的两个Boss是否都已死亡  
            var witherExists = level.EntityExists(e => e.IsEntityOf(VanillaBossID.wither) && e.IsHostileEntity() && !e.IsDead);
            var seijaExists = level.EntityExists(e => e.IsEntityOf(VanillaBossID.seija) && e.IsHostileEntity() && !e.IsDead);

            // 只有当两个Boss都死亡时才进入胜利阶段  
            if (!witherExists && !seijaExists)
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
