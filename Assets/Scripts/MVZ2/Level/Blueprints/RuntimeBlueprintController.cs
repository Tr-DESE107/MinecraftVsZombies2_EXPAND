﻿using MVZ2.Level.UI;
using MVZ2.UI;
using MVZ2.Vanilla;
using MVZ2.Vanilla.Level;
using MVZ2.Vanilla.SeedPacks;
using MVZ2Logic;
using PVZEngine.Definitions;
using PVZEngine.SeedPacks;
using UnityEngine;

namespace MVZ2.Level
{
    public abstract class RuntimeBlueprintController : BlueprintController
    {
        #region 公有方法
        public RuntimeBlueprintController(ILevelController controller, Blueprint ui, int index, SeedPack seedPack) : base(controller, ui, index)
        {
            SeedPack = seedPack;
            ui.gameObject.name = seedPack.GetDefinitionID().ToString();
            seedPack.SetModelInterface(modelInterface);
        }
        public override void Remove()
        {
            base.Remove();
            SeedPack.SetModelInterface(null);
        }
        public void ForceUpdateBlueprintHotkeyText()
        {
            UpdateHotkeyText();
        }
        protected override void AddCallbacks()
        {
            base.AddCallbacks();
            SeedPack.OnDefinitionChanged += OnDefinitionChangedCallback;
        }
        protected override void RemoveCallbacks()
        {
            base.RemoveCallbacks();
            SeedPack.OnDefinitionChanged -= OnDefinitionChangedCallback;
        }
        public virtual void UpdateFixed()
        {
            var model = GetModel();
            if (model)
            {
                model.UpdateFixed();
            }
        }
        public virtual void UpdateFrame(float deltaTime)
        {
            UpdateView();
            var model = GetModel();
            if (model)
            {
                model.UpdateFrame(deltaTime);
                model.UpdateAnimators(deltaTime);
            }
            if (lastIndex != Index)
            {
                lastIndex = Index;
                UpdateHotkeyText();
            }
        }
        public override BlueprintViewData GetBlueprintViewData()
        {
            return Main.ResourceManager.GetBlueprintViewData(SeedPack);
        }
        public bool CanPick()
        {
            return CanPick(out _);
        }
        public bool CanPick(out string errorMessage)
        {
            return SeedPack.CanPick(out errorMessage);
        }
        public override TooltipViewData GetTooltipViewData()
        {
            var viewData = base.GetTooltipViewData();
            viewData.error = GetTooltipErrorMessage();
            return viewData;
        }
        public override SeedDefinition GetSeedDefinition()
        {
            return SeedPack.Definition;
        }
        #endregion

        #region 私有方法

        #region 事件回调
        private void OnDefinitionChangedCallback(SeedDefinition seedDef)
        {
            ui.UpdateView(Main.ResourceManager.GetBlueprintViewData(SeedPack));
        }
        #endregion

        protected virtual bool ShouldBlueprintTwinkle(SeedPack seedPack)
        {
            if (SeedPack.IsTwinkling())
            {
                return true;
            }
            else if (Level.IsHoldingTrigger() && SeedPack.CanInstantTrigger())
            {
                return true;
            }
            else if (Level.IsHoldingStarshard() && SeedPack.CanInstantEvoke())
            {
                return true;
            }
            return false;
        }
        private string GetTooltipErrorMessage()
        {
            if (!CanPick(out var errorMessage) && !string.IsNullOrEmpty(errorMessage))
            {
                return Main.LanguageManager._p(VanillaStrings.CONTEXT_BLUEPRINT_ERROR, errorMessage);
            }
            return string.Empty;
        }
        private void UpdateHotkeyText()
        {
            var name = GetHotkeyName();
            ui.SetHotkeyText(name);
        }
        private string GetHotkeyName()
        {
            if (Global.IsMobile() || !Main.OptionsManager.ShowHotkeyIndicators())
                return string.Empty;
            var hotkey = Main.OptionsManager.GetBlueprintKeyBinding(Index);
            return hotkey != KeyCode.None ? Main.InputManager.GetKeyCodeName(hotkey) : string.Empty;
        }
        #endregion

        #region 序列化
        public SerializableBlueprintController ToSerializable()
        {
            var serializable = CreateSerializable();
            var model = GetModel();
            if (model != null)
            {
                serializable.model = model.ToSerializable();
            }
            return serializable;
        }
        public void LoadFromSerializable(SerializableBlueprintController serializable)
        {
            var model = GetModel();
            if (model != null && serializable.model != null)
            {
                model.LoadFromSerializable(serializable.model);
            }
            LoadSerializable(serializable);
        }
        protected abstract SerializableBlueprintController CreateSerializable();
        protected virtual void LoadSerializable(SerializableBlueprintController serializable) { }

        #endregion

        #region 属性字段
        public SeedPack SeedPack { get; private set; }
        private int lastIndex = -1;
        #endregion
    }
    public struct BlueprintPickupInfo
    {
        public bool instantTrigger;
        public bool instantEvoke;
    }
}
