using MVZ2.GameContent.Buffs.Enemies;
using MVZ2.GameContent.Buffs.Level;
using MVZ2.Vanilla;
using MVZ2.Vanilla.Audios;
using MVZ2Logic.Level;
using PVZEngine;
using PVZEngine.Callbacks;
using PVZEngine.Definitions;
using PVZEngine.Entities;
using PVZEngine.Level;
using Tools;

namespace MVZ2.GameContent.Stages
{
    [Definition(VanillaStageNames.castle6)]
    public partial class LittleZombieStage : StageDefinition
    {
        public LittleZombieStage(string nsp, string name) : base(nsp, name)
        {
            AddBehaviour(new WaveStageBehaviour(this));
            AddBehaviour(new GemStageBehaviour(this));
            AddBehaviour(new StarshardStageBehaviour(this));
            AddBehaviour(new ConveyorStageBehaviour(this));

            AddTrigger(LevelCallbacks.POST_ENTITY_INIT, PostEnemyInitCallback, filter: EntityTypes.ENEMY);
        }
        public override void OnStart(LevelEngine level)
        {
            base.OnStart(level);
            level.SetConveyorMode(true);
            if (!level.HasBuff<LittleZombieLevelBuff>())
            {
                level.AddBuff<LittleZombieLevelBuff>();
            }
        }
        public override void OnPostWave(LevelEngine level, int wave)
        {
            base.OnPostWave(level, wave);
            if (wave == 11) 
            {
                level.PlaySound(VanillaSoundID.growBig);
            }
        }
        public void PostEnemyInitCallback(Entity entity)
        {
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
            entity.Health = entity.GetMaxHealth();
        }
        public static int GetBigCounter(LevelEngine level) => level.GetBehaviourField<int>(ID, FIELD_BIG_COUNTER);
        public static void SetBigCounter(LevelEngine level, int value) => level.SetBehaviourField(ID, FIELD_BIG_COUNTER, value);

        public static readonly NamespaceID ID = new NamespaceID(VanillaMod.spaceName, "little_zombie_stage");
        public const string FIELD_BIG_COUNTER = "BigCounter";
        public const int MAX_BIG_COUNTER = 6;
    }
}
