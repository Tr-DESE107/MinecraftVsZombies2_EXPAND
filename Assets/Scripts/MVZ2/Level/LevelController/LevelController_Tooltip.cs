using MVZ2.Level.UI;
using MVZ2.Vanilla;
using MVZ2.Vanilla.Level;
using MVZ2Logic;
using PVZEngine;

namespace MVZ2.Level
{
    public partial class LevelController
    {
        private void Awake_Tooltip()
        {
            pickaxeTooltipSource = new PickaxeTooltipSource(this);
            triggerTooltipSource = new TriggerTooltipSource(this);

            var uiPreset = GetUIPreset();
            uiPreset.HideTooltip();
        }
        public void ShowTooltip(ITooltipSource source)
        {
            tooltipSource = source;
            var target = tooltipSource.GetTarget(this);
            if (!target.Anchor || !target.Anchor.isActiveAndEnabled || target.Anchor.IsDisabled)
                return;
            var uiPreset = GetUIPreset();
            uiPreset.ShowTooltip();
        }
        public void UpdateTooltip()
        {
            if (tooltipSource == null)
                return;
            var uiPreset = GetUIPreset();
            uiPreset.UpdateTooltip(tooltipSource.GetTarget(this), tooltipSource.GetViewData(this));
        }
        public void HideTooltip()
        {
            tooltipSource = null;
            var uiPreset = GetUIPreset();
            uiPreset.HideTooltip();
        }

        #region 属性字段
        private ITooltipSource tooltipSource;
        private ITooltipSource pickaxeTooltipSource;
        private ITooltipSource triggerTooltipSource;
        #endregion

        private class PickaxeTooltipSource : ITooltipSource
        {
            private LevelController controller;
            public PickaxeTooltipSource(LevelController level)
            {
                this.controller = level;
            }
            public ITooltipTarget GetTarget(LevelController level)
            {
                return level.GetUIPreset().GetPickaxeSlot();
            }
            public TooltipViewData GetViewData(LevelController level)
            {
                string error = null;
                if (!level.level.CanUsePickaxe())
                {
                    var disableID = controller.level.GetPickaxeDisableID();
                    var message = Global.Game.GetBlueprintErrorMessage(disableID);
                    if (!string.IsNullOrEmpty(message))
                    {
                        error = level.Localization._p(VanillaStrings.CONTEXT_BLUEPRINT_ERROR, message);
                    }
                }
                return new TooltipViewData()
                {
                    name = level.Localization._(VanillaStrings.TOOLTIP_DIG_CONTRAPTION),
                    error = error,
                    description = null
                };
            }
        }
        private class TriggerTooltipSource : ITooltipSource
        {
            private LevelController controller;
            public TriggerTooltipSource(LevelController level)
            {
                this.controller = level;
            }
            public ITooltipTarget GetTarget(LevelController level)
            {
                return level.GetUIPreset().GetCurrentTriggerUI();
            }
            public TooltipViewData GetViewData(LevelController level)
            {
                string error = null;
                if (level.level.CanUseTrigger())
                {
                    var disableID = level.level.GetTriggerDisableID();
                    var message = Global.Game.GetBlueprintErrorMessage(disableID);
                    if (!string.IsNullOrEmpty(message))
                    {
                        error = controller.Localization._p(VanillaStrings.CONTEXT_BLUEPRINT_ERROR, message);
                    }
                }
                return new TooltipViewData()
                {
                    name = controller.Localization._(VanillaStrings.TOOLTIP_TRIGGER_CONTRAPTION),
                    error = error,
                    description = null
                };
            }
        }
        private class ArtifactTooltipSource : ITooltipSource
        {
            private LevelController controller;
            private NamespaceID artifactID;
            private ITooltipTarget target;

            public ArtifactTooltipSource(LevelController controller, NamespaceID artifactID, ITooltipTarget target)
            {
                this.controller = controller;
                this.artifactID = artifactID;
                this.target = target;
            }

            public ITooltipTarget GetTarget(LevelController level)
            {
                return target;
            }
            public TooltipViewData GetViewData(LevelController level)
            {
                var main = controller.Main;
                var name = main.ResourceManager.GetArtifactName(artifactID);
                var tooltip = main.ResourceManager.GetArtifactTooltip(artifactID);
                return new TooltipViewData()
                {
                    name = name,
                    error = string.Empty,
                    description = tooltip
                };
            }
        }
    }
    public interface ITooltipSource
    {
        ITooltipTarget GetTarget(LevelController level);
        TooltipViewData GetViewData(LevelController level);
    }
}