using MVZ2.Level.UI;
using MVZ2.UI;
using PVZEngine.Definitions;

namespace MVZ2.Level
{
    public class ChosenBlueprintController : BlueprintController
    {
        public ChosenBlueprintController(ILevelController controller, Blueprint ui, int index, SeedDefinition definition) : base(controller, ui, index)
        {
            Definition = definition;
            ui.gameObject.name = definition.GetID().ToString();
            ui.SetDisabled(false);
            ui.SetRecharge(0);
            ui.SetSelected(false);
            ui.SetTwinkling(false);
        }
        protected override void OnDestroy()
        {
            Controller.BlueprintChoosePart.DestroyChosenBlueprintUIAt(Index);
        }
        public override TooltipViewData GetTooltipViewData()
        {
            var tooltip = base.GetTooltipViewData();
            var id = Definition.GetID();
            tooltip.error = Controller.BlueprintChoosePart.GetChosenBlueprintTooltipError(Index);
            tooltip.description = Controller.BlueprintChoosePart.GetBlueprintTooltip(id, IsCommandBlock());
            return tooltip;
        }
        public override bool IsCommandBlock()
        {
            return Controller.BlueprintChoosePart.IsChosenBlueprintCommandBlock(Index);
        }
        public override void Click()
        {
            base.Click();
            if (!Controller.CanChooseBlueprints())
                return;
            Controller.BlueprintChoosePart.UnchooseBlueprint(Index);
        }
        public override SeedDefinition GetSeedDefinition()
        {
            return Definition;
        }
        #region 事件回调
        #endregion
        public SeedDefinition Definition { get; private set; }
    }
}
