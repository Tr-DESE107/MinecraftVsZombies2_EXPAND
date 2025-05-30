using MVZ2.UI;
using MVZ2.Vanilla.Level;
using MVZ2.Vanilla.SeedPacks;
using PVZEngine.SeedPacks;
using UnityEngine;

namespace MVZ2.Level
{
    public class ConveyorBlueprintController : RuntimeBlueprintController
    {
        public ConveyorBlueprintController(ILevelController level, Blueprint ui, int index, SeedPack seedPack) : base(level, ui, index, seedPack)
        {
            Position = level.GetEngine().GetConveyorSlotCount();
        }
        protected override void OnDestroy()
        {
            Controller.BlueprintController.DestroyConveyorBlueprintAt(Index);
        }
        public override BlueprintViewData GetBlueprintViewData()
        {
            BlueprintViewData viewData = Main.ResourceManager.GetBlueprintViewData(SeedPack);
            viewData.cost = string.Empty;
            return viewData;
        }
        public override void UpdateFixed()
        {
            base.UpdateFixed();
            var index = Index;
            Position -= Controller.BlueprintController.GetConveyorSpeed() / 45f;
            float minPosition = 0;
            if (index > 0)
            {
                var prevSeed = Controller.BlueprintController.GetConveyorBlueprintController(index - 1);
                minPosition = prevSeed.Position + 1;
            }
            Position = Mathf.Max(Position, minPosition);
        }
        public override void UpdateFrame(float deltaTime)
        {
            base.UpdateFrame(deltaTime);

            ui.SetRecharge(0);
            ui.SetDisabled(false);
            ui.SetTwinkling(ShouldBlueprintTwinkle(SeedPack));
            ui.SetSelected(Level.IsHoldingConveyorBlueprint(Index));

            Controller.BlueprintController.SetConveyorBlueprintUIPosition(Index, Position);
        }
        public override bool IsCommandBlock()
        {
            return SeedPack.IsCommandBlock();
        }
        protected override SerializableBlueprintController CreateSerializable()
        {
            return new SerializableConveyorBlueprintController()
            {
                position = Position
            };
        }
        protected override void LoadSerializable(SerializableBlueprintController serializable)
        {
            if (serializable is not SerializableConveyorBlueprintController conveyor)
                return;
            Position = conveyor.position;
        }
        public float Position { get; set; }
    }
    public class SerializableConveyorBlueprintController : SerializableBlueprintController
    {
        public float position;
    }
}
