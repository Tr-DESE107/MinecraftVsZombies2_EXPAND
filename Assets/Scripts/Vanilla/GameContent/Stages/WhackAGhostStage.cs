using MVZ2.GameContent.HeldItems;
using MVZ2.Vanilla.HeldItems;
using MVZ2.Vanilla.Level;
using MVZ2Logic.Level;
using PVZEngine.Definitions;
using PVZEngine.Entities;
using PVZEngine.Level;

namespace MVZ2.GameContent.Stages
{
    public partial class WhackAGhostStage : StageDefinition
    {
        public WhackAGhostStage(string nsp, string name) : base(nsp, name)
        {
            SetProperty(VanillaAreaProps.DARKNESS_VALUE, 0.5f);
            AddBehaviour(new WaveStageBehaviour(this));
            AddBehaviour(new GemStageBehaviour(this));
        }

        public override void OnUpdate(LevelEngine level)
        {
            base.OnUpdate(level);
            if (level.GetHeldItemType() == BuiltinHeldTypes.none)
            {
                level.SetHeldItem(VanillaHeldTypes.sword, 0, 255, true);
            }
        }
    }
}
