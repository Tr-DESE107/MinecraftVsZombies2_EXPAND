using MVZ2.Vanilla.Level;
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
            var waveTimer = new FrameTimer(CONVEYOR_INTERVAL);
            SetConveyorTimer(level, waveTimer);
        }
        public override void Update(LevelEngine level)
        {
            var conveyorTimer = GetConveyorTimer(level);
            conveyorTimer.Run(level.GetConveySpeed());
            if (conveyorTimer.Expired)
            {
                level.ConveyRandomSeedPack();
                conveyorTimer.Reset();
            }
        }

        #region 关卡属性
        public FrameTimer GetConveyorTimer(LevelEngine level) => level.GetProperty<FrameTimer>("ConveyorTimer");
        public void SetConveyorTimer(LevelEngine level, FrameTimer value) => level.SetProperty("ConveyorTimer", value);
        #endregion

        #region 属性字段
        public const string PROP_CONVEYOR_TIMER = "ConveyorTimer";
        public const int CONVEYOR_INTERVAL = 120;
        #endregion
    }
}
