﻿using MVZ2.Level.UI;
using MVZ2.Managers;
using MVZ2.Models;
using MVZ2.SeedPacks;
using MVZ2.UI;
using MVZ2.Vanilla.Level;
using MVZ2Logic;
using PVZEngine.Definitions;
using PVZEngine.Level;
using PVZEngine.Models;
using UnityEngine.EventSystems;

namespace MVZ2.Level
{
    public abstract class BlueprintController
    {
        #region 公有方法
        public BlueprintController(ILevelController controller, Blueprint ui, int index)
        {
            Controller = controller;
            this.ui = ui;
            Index = index;

            modelInterface = new BlueprintModelInterface(this);
            tooltipSource = new BlueprintTooltipSource(this);
        }
        public void Init()
        {
            UpdateView();
            var model = GetModel();
            if (model != null)
            {
                model.Init(Controller.GetCamera());
            }
            AddCallbacks();
        }
        public Model GetModel()
        {
            return ui.Model;
        }
        public void UpdateView()
        {
            BlueprintViewData viewData = GetBlueprintViewData();
            ui.UpdateView(viewData);
        }
        public virtual void Remove()
        {
            RemoveCallbacks();
        }
        public void Destroy()
        {
            Remove();
            OnDestroy();
        }
        public virtual BlueprintViewData GetBlueprintViewData()
        {
            return Main.ResourceManager.GetBlueprintViewData(GetSeedDefinition(), Level.IsEndless(), IsCommandBlock());
        }
        public virtual TooltipViewData GetTooltipViewData()
        {
            return new TooltipViewData()
            {
                name = GetName(),
                error = string.Empty,
                description = string.Empty
            };
        }
        public abstract SeedDefinition GetSeedDefinition();
        public abstract bool IsCommandBlock();
        #endregion

        #region 私有方法

        #region 事件回调
        private void OnPointerInteractionCallback(Blueprint blueprint, PointerEventData eventData, PointerInteraction interaction)
        {
            switch (interaction)
            {
                case PointerInteraction.Enter:
                    Controller.ShowTooltip(tooltipSource);
                    break;
                case PointerInteraction.Exit:
                    Controller.HideTooltip();
                    break;
            }
        }
        #endregion

        protected virtual void AddCallbacks()
        {
            ui.OnPointerInteraction += OnPointerInteractionCallback;
        }
        protected virtual void RemoveCallbacks()
        {
            ui.OnPointerInteraction -= OnPointerInteractionCallback;
        }
        protected abstract void OnDestroy();
        private string GetName()
        {
            return Controller.BlueprintChoosePart.GetBlueprintName(GetSeedDefinition()?.GetID(), IsCommandBlock());
        }
        #endregion

        #region 属性字段
        public MainManager Main => MainManager.Instance;
        public LevelEngine Level => Controller.GetEngine();
        public int Index { get; set; }
        public ILevelController Controller { get; private set; }
        protected Blueprint ui;
        protected IModelInterface modelInterface;
        protected ITooltipSource tooltipSource;
        #endregion
        private class BlueprintTooltipSource : ITooltipSource
        {
            public BlueprintTooltipSource(BlueprintController blueprintController)
            {
                this.blueprintController = blueprintController;
            }
            public ITooltipTarget GetTarget(LevelController level)
            {
                return blueprintController.ui;
            }
            public TooltipViewData GetViewData(LevelController level)
            {
                return blueprintController.GetTooltipViewData();
            }
            private BlueprintController blueprintController;
        }
    }
    public class SerializableBlueprintController
    {
        public SerializableModelData model;
    }
}
