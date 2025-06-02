﻿using MVZ2.Level.UI;
using MVZ2.UI;
using MVZ2Logic;
using PVZEngine.Definitions;
using UnityEngine.EventSystems;

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
            ui.SetTwinkleAlpha(0);
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
        protected override void AddCallbacks()
        {
            base.AddCallbacks();
            ui.OnPointerInteraction += OnPointerInteractionCallback;
        }
        protected override void RemoveCallbacks()
        {
            base.RemoveCallbacks();
            ui.OnPointerInteraction -= OnPointerInteractionCallback;
        }
        private void OnPointerInteractionCallback(Blueprint blueprint, PointerEventData eventData, PointerInteraction interaction)
        {
            if (interaction != PointerInteraction.Down)
                return;
            if (!Controller.CanChooseBlueprints())
                return;
            Controller.BlueprintChoosePart.UnchooseBlueprint(Index);
        }
        public override SeedDefinition GetSeedDefinition()
        {
            return Definition;
        }
        public SeedDefinition Definition { get; private set; }
    }
}
