using MVZ2.Vanilla.Level;
using MVZ2.Vanilla.Properties;
using MVZ2Logic.Level;
using PVZEngine;
using PVZEngine.Callbacks;
using PVZEngine.Definitions;
using PVZEngine.Level;
using Tools;

namespace MVZ2.GameContent.Stages
{
    public class ConveyorStageBehaviour : StageBehaviour
    {
        public ConveyorStageBehaviour(StageDefinition stageDef) : base(stageDef)
        {
            stageDef.SetProperty(VanillaLevelProps.CONVEY_SPEED, 1);
        }
        public override void Start(LevelEngine level)
        {
            level.SetConveyorMode(true);
            var waveTimer = new FrameTimer(CONVEYOR_INTERVAL);
            SetConveyorTimer(level, waveTimer);
        }
        public override void Update(LevelEngine level)
        {
            if (level.IsCleared)
                return;
            var conveyorTimer = GetConveyorTimer(level);
            conveyorTimer.Run(level.GetConveySpeed());
            if (conveyorTimer.Expired)
            {
                level.ConveyRandomSeedPack();
                conveyorTimer.Reset();
            }
        }

        #region 关卡属性
        public FrameTimer GetConveyorTimer(LevelEngine level) => level.GetBehaviourField<FrameTimer>(PROP_CONVEYOR_TIMER);
        public void SetConveyorTimer(LevelEngine level, FrameTimer value) => level.SetBehaviourField(PROP_CONVEYOR_TIMER, value);
        #endregion

        #region 属性字段
        public const string PROP_REGION = "conveyor";
        [LevelPropertyRegistry(PROP_REGION)]
        public static readonly VanillaLevelPropertyMeta PROP_CONVEYOR_TIMER = new VanillaLevelPropertyMeta("ConveyorTimer");
        public const int CONVEYOR_INTERVAL = 120;
        #endregion
    }
}
