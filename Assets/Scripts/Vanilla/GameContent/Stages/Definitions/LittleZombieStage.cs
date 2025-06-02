﻿using MVZ2.GameContent.Buffs.Enemies;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Properties;
using MVZ2Logic.Level;
using PVZEngine;
using PVZEngine.Callbacks;
using PVZEngine.Definitions;
using PVZEngine.Entities;
using PVZEngine.Level;

namespace MVZ2.GameContent.Stages
{
    public partial class LittleZombieStage : StageDefinition
    {
        public LittleZombieStage(string nsp, string name) : base(nsp, name)
        {
            AddBehaviour(new WaveStageBehaviour(this));
            AddBehaviour(new FinalWaveClearBehaviour(this));
            AddBehaviour(new GemStageBehaviour(this));
            AddBehaviour(new StarshardStageBehaviour(this));
            AddBehaviour(new ConveyorStageBehaviour(this));

            AddTrigger(LevelCallbacks.POST_ENTITY_INIT, PostEnemyInitCallback, filter: EntityTypes.ENEMY);
        }
        public override void OnPostWave(LevelEngine level, int wave)
        {
            base.OnPostWave(level, wave);
            if (wave == 11)
            {
                level.PlaySound(VanillaSoundID.growBig);
            }
        }
        public void PostEnemyInitCallback(EntityCallbackParams param, CallbackResult result)
        {
            var entity = param.entity;
            var level = entity.Level;
            if (level.StageDefinition != this)
                return;
            bool big = false;
            if (level.CurrentWave > 10)
            {
                var bigCounter = GetBigCounter(level);
                if (bigCounter > MAX_BIG_COUNTER)
                {
                    bigCounter -= MAX_BIG_COUNTER;
                    big = true;
                }
                else
                {
                    bigCounter++;
                }
                SetBigCounter(level, bigCounter);
            }
            if (big)
            {
                entity.AddBuff<BigTroubleBuff>();
            }
            else
            {
                entity.AddBuff<LittleZombieBuff>();
            }
        }
        public static int GetBigCounter(LevelEngine level) => level.GetBehaviourField<int>(FIELD_BIG_COUNTER);
        public static void SetBigCounter(LevelEngine level, int value) => level.SetBehaviourField(FIELD_BIG_COUNTER, value);

        public const string REGION_NAME = "little_zombie_stage";
        [LevelPropertyRegistry(REGION_NAME)]
        public static readonly VanillaLevelPropertyMeta<int> FIELD_BIG_COUNTER = new VanillaLevelPropertyMeta<int>("BigCounter");
        public const int MAX_BIG_COUNTER = 6;
    }
}
