using System.Linq;
using MVZ2.GameContent.Difficulties;
using MVZ2.GameContent.Enemies;
using MVZ2.GameContent.Pickups;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Callbacks;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Level;
using MVZ2Logic.Level;
using PVZEngine;
using PVZEngine.Armors;
using PVZEngine.Definitions;
using PVZEngine.Entities;
using PVZEngine.Level;
using Tools;
using UnityEngine;

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
