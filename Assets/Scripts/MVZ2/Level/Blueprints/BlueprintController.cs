using MVZ2.Level.UI;
using MVZ2.Managers;
using MVZ2.Models;
using MVZ2.SeedPacks;
using MVZ2.UI;
using MVZ2.Vanilla.SeedPacks;
using MVZ2Logic.SeedPacks;
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
        public virtual void Click() { }
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
            return Main.ResourceManager.GetBlueprintViewData(GetSeedDefinition());
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
        #endregion

        #region 私有方法

        #region 事件回调
        private void OnPointerEnterCallback(Blueprint blueprint, PointerEventData eventData)
        {
            var tooltipViewData = GetTooltipViewData();
            Controller.ShowTooltipOnComponent(ui, tooltipViewData);
        }
        private void OnPointerExitCallback(Blueprint blueprint, PointerEventData eventData)
        {
            Controller.HideTooltip();
        }
        private void OnPointerDownCallback(Blueprint blueprint, PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;
            Click();
        }
        #endregion

        protected virtual void AddCallbacks()
        {
            ui.OnPointerDown += OnPointerDownCallback;
            ui.OnPointerEnter += OnPointerEnterCallback;
            ui.OnPointerExit += OnPointerExitCallback;
        }
        protected virtual void RemoveCallbacks()
        {
            ui.OnPointerDown -= OnPointerDownCallback;
            ui.OnPointerEnter -= OnPointerEnterCallback;
            ui.OnPointerExit -= OnPointerExitCallback;
        }
        protected abstract void OnDestroy();
        private string GetName()
        {
            var definition = GetSeedDefinition();
            if (definition == null)
                return string.Empty;
            var seedType = definition.GetSeedType();
            if (seedType == SeedTypes.ENTITY)
            {
                var entityID = definition.GetSeedEntityID();
                return Main.ResourceManager.GetEntityName(entityID);
            }
            else if (seedType == SeedTypes.OPTION)
            {
                var optionID = definition.GetSeedOptionID();
                return Main.ResourceManager.GetSeedOptionName(optionID);
            }
            return string.Empty;
        }
        #endregion

        #region 属性字段
        public MainManager Main => MainManager.Instance;
        public LevelEngine Level => Controller.GetEngine();
        public int Index { get; set; }
        public ILevelController Controller { get; private set; }
        protected Blueprint ui;
        protected IModelInterface modelInterface;
        #endregion
    }
    public class SerializableBlueprintController
    {
        public SerializableModelData model;
    }
}
