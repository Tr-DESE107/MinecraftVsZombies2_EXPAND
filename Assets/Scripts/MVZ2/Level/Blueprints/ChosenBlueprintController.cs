using System;
using System.Reflection;
using MVZ2.GameContent.Effects;
using MVZ2.Level.UI;
using MVZ2.Managers;
using MVZ2.Models;
using MVZ2.UI;
using MVZ2.Vanilla;
using MVZ2.Vanilla.SeedPacks;
using MVZ2Logic.Level;
using MVZ2Logic.SeedPacks;
using PVZEngine;
using PVZEngine.Definitions;
using PVZEngine.Level;
using PVZEngine.Models;
using PVZEngine.SeedPacks;
using UnityEngine;
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
            tooltip.error = Controller.BlueprintChoosePart.GetBlueprintTooltipError(id);
            tooltip.description = Controller.BlueprintChoosePart.GetBlueprintTooltip(id);
            return tooltip;
        }
        public override void Click()
        {
            base.Click();
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
