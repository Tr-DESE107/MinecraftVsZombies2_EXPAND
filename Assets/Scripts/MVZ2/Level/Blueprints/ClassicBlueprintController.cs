using MVZ2.UI;
using MVZ2.Vanilla.Level;
using MVZ2.Vanilla.SeedPacks;
using PVZEngine.Level;
using PVZEngine.SeedPacks;

namespace MVZ2.Level
{
    public class ClassicBlueprintController : RuntimeBlueprintController
    {
        public ClassicBlueprintController(ILevelController level, Blueprint ui, int index, SeedPack seedPack) : base(level, ui, index, seedPack)
        {
        }
        protected override void OnDestroy()
        {
            Controller.BlueprintController.DestroyClassicBlueprintAt(Index);
        }
        public override void UpdateFrame(float deltaTime)
        {
            base.UpdateFrame(deltaTime);

            var maxCharge = SeedPack.GetMaxRecharge();
            ui.SetRecharge(maxCharge == 0 ? 0 : 1 - SeedPack.GetRecharge() / maxCharge);
            ui.SetDisabled(!CanPick());
            ui.SetTwinkling(ShouldBlueprintTwinkle(SeedPack));
            ui.SetSelected(Level.IsHoldingClassicBlueprint(Index));
        }
        public override bool IsCommandBlock()
        {
            return SeedPack.IsCommandBlock();
        }
        protected override SerializableBlueprintController CreateSerializable()
        {
            return new SerializableClassicBlueprintController()
            {
            };
        }
        protected override void LoadSerializable(SerializableBlueprintController serializable)
        {
        }
    }
    public class SerializableClassicBlueprintController : SerializableBlueprintController
    {
    }
}
