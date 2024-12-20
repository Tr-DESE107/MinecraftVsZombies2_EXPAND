using System;
using System.Collections.Generic;
using System.Reflection;
using MVZ2.GameContent.HeldItems;
using MVZ2.Level.Components;
using MVZ2.Level.UI;
using MVZ2.UI;
using MVZ2.Vanilla;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.HeldItems;
using MVZ2.Vanilla.Level;
using MVZ2.Vanilla.SeedPacks;
using MVZ2Logic;
using MVZ2Logic.Level;
using MVZ2Logic.SeedPacks;
using PVZEngine;
using PVZEngine.Definitions;
using PVZEngine.Level;
using PVZEngine.SeedPacks;
using UnityEngine;
using UnityEngine.EventSystems;
using static MVZ2.Level.LevelController;

namespace MVZ2.Level
{
    public partial class LevelController : MonoBehaviour, IDisposable
    {
        public void SetConveyorMode(bool mode)
        {
            var uiPreset = GetUIPreset();
            uiPreset.SetConveyorMode(mode);
            isConveyorMode = mode;
        }
        #region 私有方法

        private void Awake_Blueprints()
        {
            classicBlueprintMode = new ClassicBlueprintMode(this);
            conveyorBlueprintMode = new ConveyorBlueprintMode(this);
            var levelUI = GetUIPreset();
            levelUI.OnBlueprintPointerEnter += UI_OnBlueprintPointerEnterCallback;
            levelUI.OnBlueprintPointerExit += UI_OnBlueprintPointerExitCallback;
            levelUI.OnBlueprintPointerDown += UI_OnBlueprintPointerDownCallback;

            levelUI.OnConveyorBlueprintPointerEnter += UI_OnConveyorBlueprintPointerEnterCallback;
            levelUI.OnConveyorBlueprintPointerExit += UI_OnConveyorBlueprintPointerExitCallback;
            levelUI.OnConveyorBlueprintPointerDown += UI_OnConveyorBlueprintPointerDownCallback;
        }

        #region 事件回调

        private void Engine_OnSeedPackChangedCallback(int index)
        {
            classicBlueprintMode.RefreshBlueprint(index);
        }
        private void Engine_OnSeedPackCountChangedCallback(int count)
        {
            UpdateClassicBlueprintCount();
        }
        private void Engine_OnConveyorSeedPackAddedCallback(int index)
        {
            conveyorSeedPositions.Insert(index, level.GetConveyorSlotCount() + 1);
            conveyorBlueprintMode.AddBlueprint(index);
        }
        private void Engine_OnConveyorSeedPackRemovedCallback(int index)
        {
            conveyorSeedPositions.RemoveAt(index);
            conveyorBlueprintMode.RemoveBlueprintAt(index);
        }
        private void Engine_OnConveyorSeedPackCountChangedCallback(int count)
        {
            UpdateConveyorBlueprintCount();
        }


        private void UI_OnBlueprintPointerEnterCallback(int index, PointerEventData eventData)
        {
            OnBlueprintEnterCallback(index, false);
        }
        private void UI_OnBlueprintPointerExitCallback(int index, PointerEventData eventData)
        {
            OnBlueprintExitCallback(index);
        }
        private void UI_OnBlueprintPointerDownCallback(int index, PointerEventData eventData)
        {
            OnBlueprintDownCallback(index, false, eventData);
        }

        private void UI_OnConveyorBlueprintPointerEnterCallback(int index, PointerEventData eventData)
        {
            OnBlueprintEnterCallback(index, true);
        }
        private void UI_OnConveyorBlueprintPointerExitCallback(int index, PointerEventData eventData)
        {
            OnBlueprintExitCallback(index);
        }
        private void UI_OnConveyorBlueprintPointerDownCallback(int index, PointerEventData eventData)
        {
            OnBlueprintDownCallback(index, true, eventData);
        }

        private void OnBlueprintEnterCallback(int index, bool conveyor)
        {
            var levelUI = GetUIPreset();
            string name;
            string tooltip;
            string error = null;
            if (IsGameStarted())
            {
                var currentBlueprintMode = GetBlueprintMode(conveyor);
                var seedPack = currentBlueprintMode.GetSeedPackAt(index);
                if (seedPack == null)
                    return;
                GetBlueprintTooltip(seedPack.Definition, out name, out tooltip);
                if (!currentBlueprintMode.CanPickBlueprint(seedPack, out var errorMessage) && !string.IsNullOrEmpty(errorMessage))
                {
                    error = Localization._(errorMessage);
                }
            }
            else
            {
                var chosenIndex = chosenBlueprints[index];
                var blueprintID = choosingBlueprints[chosenIndex];
                GetBlueprintTooltip(blueprintID, out name, out tooltip);
                if (!IsChoosingBlueprintError(blueprintID, out var errorMessage) && !string.IsNullOrEmpty(errorMessage))
                {
                    error = Localization._(errorMessage);
                }
            }
            var viewData = new TooltipViewData()
            {
                name = name,
                error = error,
                description = tooltip
            };
            if (conveyor)
            {
                levelUI.ShowTooltipOnConveyorBlueprint(index, viewData);
            }
            else
            {
                levelUI.ShowTooltipOnBlueprint(index, viewData);
            }
        }
        private void OnBlueprintExitCallback(int index)
        {
            var levelUI = GetUIPreset();
            levelUI.HideTooltip();
        }
        private void OnBlueprintDownCallback(int index, bool conveyor, PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;
            var currentBlueprintMode = GetBlueprintMode(conveyor);
            currentBlueprintMode.ClickBlueprint(index);
        }
        #endregion

        #region 选卡
        private void UnchooseBlueprint(int index)
        {
            var choosingIndex = chosenBlueprints[index];
            chosenBlueprints.RemoveAt(index);

            var uiPreset = GetUIPreset();
            var blueprintUI = uiPreset.GetBlueprintAt(index);
            uiPreset.RemoveBlueprintAt(index);

            var startPos = blueprintUI.transform.position;
            var targetBlueprint = uiPreset.GetBlueprintChooseItem(choosingIndex);
            var movingBlueprint = uiPreset.CreateMovingBlueprint();
            movingBlueprint.transform.position = startPos;
            movingBlueprint.SetBlueprint(blueprintUI);
            movingBlueprint.SetMotion(startPos, targetBlueprint.transform);
            movingBlueprint.OnMotionFinished += () =>
            {
                UpdateBlueprintChooseItem(choosingIndex);
                uiPreset.RemoveMovingBlueprint(movingBlueprint);
            };

            level.PlaySound(VanillaSoundID.tap);
            uiPreset.HideTooltip();
        }
        #endregion
        private void UpdateConveyorBlueprintPositions()
        {
            for (int i = 0; i < conveyorSeedPositions.Count; i++)
            {
                var position = conveyorSeedPositions[i];
                position -= conveyorSpeed / 45f;
                var minPosition = i == 0 ? 0 : conveyorSeedPositions[i - 1] + 1;
                position = Mathf.Max(position, minPosition);
                conveyorSeedPositions[i] = position;
            }
        }
        private float GetConveyorBlueprintPosition(int index)
        {
            if (index < 0 || index >= conveyorSeedPositions.Count)
                return 0;
            return conveyorSeedPositions[index];
        }

        private void InitBlueprints()
        {
            var blueprintMode = GetCurrentBlueprintMode();
            blueprintMode.InitBlueprintsUI();
        }
        private void UpdateBlueprintsState()
        {
            var blueprintMode = GetCurrentBlueprintMode();
            blueprintMode.UpdateBlueprintsUI();
        }
        private BlueprintMode GetBlueprintMode(bool conveyor)
        {
            return conveyor ? conveyorBlueprintMode : classicBlueprintMode;
        }
        private BlueprintMode GetCurrentBlueprintMode()
        {
            return GetBlueprintMode(isConveyorMode);
        }
        private void UpdateClassicBlueprintCount()
        {
            var count = level.GetSeedSlotCount();
            var levelUI = GetUIPreset();
            levelUI.SetBlueprintSlotCount(count);
        }
        private void UpdateConveyorBlueprintCount()
        {
            var count = level.GetConveyorSlotCount();
            var levelUI = GetUIPreset();
            levelUI.SetConveyorSlotCount(count);
        }
        private Sprite GetBlueprintIcon(int i)
        {
            var seeds = level.GetAllSeedPacks();
            var seed = seeds[i];
            return GetBlueprintIcon(seed);
        }
        private Sprite GetBlueprintIcon(SeedPack seed)
        {
            if (seed == null)
                return null;
            return Resources.GetBlueprintIcon(seed.Definition);
        }
        private void GetBlueprintTooltip(NamespaceID blueprintID, out string name, out string tooltip)
        {
            var definition = Main.Game.GetSeedDefinition(blueprintID);
            GetBlueprintTooltip(definition, out name, out tooltip);
        }
        private void GetBlueprintTooltip(SeedDefinition definition, out string name, out string tooltip)
        {
            name = string.Empty;
            tooltip = string.Empty;
            if (definition == null || definition.GetSeedType() != SeedTypes.ENTITY)
                return;
            var entityID = definition.GetSeedEntityID();
            name = Resources.GetEntityName(entityID);
            tooltip = Resources.GetEntityTooltip(entityID);
        }
        private bool IsTriggerSwapped()
        {
            return Main.SaveManager.IsUnlocked(VanillaUnlockID.trigger) && Main.OptionsManager.IsTriggerSwapped();
        }

        #endregion

        private bool isConveyorMode;
        private ConveyorBlueprintMode conveyorBlueprintMode;
        private ClassicBlueprintMode classicBlueprintMode;
        private List<float> conveyorSeedPositions = new List<float>();
        [SerializeField]
        private float conveyorSpeed = 1;

        #region 内嵌类

        private abstract class BlueprintMode
        {
            #region 构造器
            public BlueprintMode(LevelController controller)
            {
                this.controller = controller;
            }
            #endregion

            #region 快捷方式
            protected LevelUIPreset GetUIPreset() => controller.GetUIPreset();
            protected bool IsTriggerSwapped() => controller.IsTriggerSwapped();
            #endregion

            #region 获取UI
            public abstract Blueprint GetBlueprintUIAt(int index);
            public abstract int GetBlueprintUIIndex(Blueprint blueprint);
            #endregion

            #region 获取种子包
            public abstract SeedPack GetSeedPackAt(int index);
            public abstract int GetSeedPackCount();
            public abstract SeedPack[] GetAllSeedPacks();
            public abstract int GetSeedSlotCount();
            #endregion

            #region 可拾取
            public abstract bool CanPickBlueprint(SeedPack seed, out string errorMessage);
            public bool CanPickBlueprint(SeedPack seed)
            {
                return CanPickBlueprint(seed, out _);
            }
            #endregion

            #region 初始化UI
            public abstract void InitBlueprintsUI();
            #endregion

            #region 更新UI
            public void UpdateBlueprintsUI()
            {
                var seeds = GetAllSeedPacks();
                for (int i = 0; i < seeds.Length; i++)
                {
                    var seed = seeds[i];
                    if (seed == null)
                        continue;
                    var classicBlueprint = GetBlueprintUIAt(i);
                    if (!classicBlueprint)
                        continue;
                    UpdateBlueprintUI(i, classicBlueprint, seed);
                }
            }
            protected abstract void UpdateBlueprintUI(int index, Blueprint blueprint, SeedPack seed);
            #endregion

            #region 操作
            public void ReleaseOnBlueprint(int index)
            {
                // 移动端会额外在手指放开在蓝图上时进行一次立即触发检测。
                if (!Global.IsMobile())
                    return;
                bool holdingTrigger = level.IsHoldingTrigger();
                bool canInstantTrigger = GetSeedPackAt(index)?.CanInstantTrigger() ?? false;
                bool usingTrigger = holdingTrigger && canInstantTrigger;
                if (!usingTrigger)
                    return;
                bool swapped = IsTriggerSwapped();
                bool instantTrigger = canInstantTrigger && (holdingTrigger != swapped);
                PickupBlueprint(index, instantTrigger, true);
            }
            public abstract void ClickBlueprint(int index);
            protected void SelectBlueprint(int index)
            {
                // 进行立即触发检测。
                bool holdingTrigger = level.IsHoldingTrigger();
                bool canInstantTrigger = GetSeedPackAt(index)?.CanInstantTrigger() ?? false;
                bool usingTrigger = holdingTrigger && canInstantTrigger;

                bool swapped = IsTriggerSwapped();
                bool instantTrigger = canInstantTrigger && (holdingTrigger != swapped);
                PickupBlueprint(index, instantTrigger, usingTrigger);
            }
            protected abstract void PickupBlueprint(int index, bool instantTrigger, bool skipCancelHeld = false);
            #endregion

            protected LevelEngine level => controller.level;
            protected LevelController controller;
        }
        private class ClassicBlueprintMode : BlueprintMode
        {
            #region 构造器
            public ClassicBlueprintMode(LevelController controller) : base(controller)
            {
            }
            #endregion

            #region 获取UI
            public override Blueprint GetBlueprintUIAt(int index)
            {
                var levelUI = GetUIPreset();
                return levelUI.GetBlueprintAt(index);
            }
            public override int GetBlueprintUIIndex(Blueprint blueprint)
            {
                var levelUI = GetUIPreset();
                return levelUI.GetBlueprintIndex(blueprint);
            }
            #endregion

            #region 获取种子包
            public override SeedPack GetSeedPackAt(int index)
            {
                return level.GetSeedPackAt(index);
            }
            public override SeedPack[] GetAllSeedPacks()
            {
                return level.GetAllSeedPacks();
            }
            public override int GetSeedPackCount()
            {
                return level.GetSeedPackCount();
            }
            public override int GetSeedSlotCount()
            {
                return level.GetSeedSlotCount();
            }
            #endregion

            #region 可拾取
            public override bool CanPickBlueprint(SeedPack seed, out string errorMessage)
            {
                if (seed == null)
                {
                    errorMessage = null;
                    return false;
                }
                if (!seed.IsCharged())
                {
                    errorMessage = Vanilla.VanillaStrings.TOOLTIP_RECHARGING;
                    return false;
                }
                if (level.Energy < seed.GetCost())
                {
                    errorMessage = Vanilla.VanillaStrings.TOOLTIP_NOT_ENOUGH_ENERGY;
                    return false;
                }
                if (seed.IsDisabled())
                {
                    errorMessage = seed.GetDisableMessage();
                    return false;
                }
                errorMessage = null;
                return true;
            }
            #endregion

            #region 初始化UI
            public override void InitBlueprintsUI()
            {
                var count = GetSeedSlotCount();
                for (int i = 0; i < count; i++)
                {
                    RefreshBlueprint(i);
                }
            }
            public void RefreshBlueprint(int index)
            {
                var seed = GetSeedPackAt(index);
                var uiPreset = GetUIPreset();
                Blueprint classicBlueprint = GetBlueprintUIAt(index);

                if (!classicBlueprint)
                {
                    classicBlueprint = uiPreset.CreateBlueprint();
                    uiPreset.InsertBlueprint(index, classicBlueprint);
                    uiPreset.ForceAlignBlueprint(index);
                }
                BlueprintViewData viewData = controller.Resources.GetBlueprintViewData(seed);
                classicBlueprint.UpdateView(viewData);
            }
            #endregion

            #region 更新UI

            protected override void UpdateBlueprintUI(int index, Blueprint blueprint, SeedPack seed)
            {
                var maxCharge = seed.GetMaxRecharge();
                blueprint.SetRecharge(maxCharge == 0 ? 0 : 1 - seed.GetRecharge() / maxCharge);
                blueprint.SetDisabled(!CanPickBlueprint(seed));
                blueprint.SetTwinkling(seed.IsTwinkling() || (level.IsHoldingTrigger() && seed.CanInstantTrigger()));
                blueprint.SetSelected(level.IsHoldingBlueprint(index));
            }
            #endregion

            #region 操作
            public override void ClickBlueprint(int index)
            {
                if (controller.CanChooseBlueprints())
                {
                    controller.UnchooseBlueprint(index);
                    return;
                }

                if (index < 0 || index >= GetSeedPackCount())
                    return;
                SelectBlueprint(index);
            }
            protected override void PickupBlueprint(int index, bool instantTrigger, bool skipCancelHeld = false)
            {
                // 先取消已经手持的物品。
                if (!skipCancelHeld)
                {
                    if (controller.TryCancelHeldItem())
                        return;
                }
                // 无法拾取蓝图。
                if (!CanPickBlueprint(GetSeedPackAt(index)))
                {
                    level.PlaySound(VanillaSoundID.buzzer);
                    return;
                }
                // 设置当前手持物品。
                level.SetHeldItem(new HeldItemStruct()
                {
                    Type = BuiltinHeldTypes.blueprint,
                    ID = index,
                    InstantTrigger = instantTrigger,
                    Priority = 0,
                });
                level.PlaySound(VanillaSoundID.pick);
            }
            #endregion
        }
        private class ConveyorBlueprintMode : BlueprintMode
        {
            #region 构造器
            public ConveyorBlueprintMode(LevelController controller) : base(controller)
            {
            }
            #endregion

            #region 获取UI
            public override Blueprint GetBlueprintUIAt(int index)
            {
                var levelUI = GetUIPreset();
                return levelUI.GetConveyorBlueprintAt(index);
            }
            public override int GetBlueprintUIIndex(Blueprint blueprint)
            {
                var levelUI = GetUIPreset();
                return levelUI.GetConveyorBlueprintIndex(blueprint);
            }
            #endregion

            #region 获取种子包
            public override SeedPack GetSeedPackAt(int index)
            {
                return level.GetConveyorSeedPackAt(index);
            }
            public override SeedPack[] GetAllSeedPacks()
            {
                return level.GetAllConveyorSeedPacks();
            }
            public override int GetSeedPackCount()
            {
                return level.GetConveyorSeedPackCount();
            }
            public override int GetSeedSlotCount()
            {
                return level.GetConveyorSlotCount();
            }
            #endregion

            #region 可拾取
            public override bool CanPickBlueprint(SeedPack seed, out string errorMessage)
            {
                errorMessage = null;
                return true;
            }
            #endregion

            #region 初始化UI
            public override void InitBlueprintsUI()
            {
                var count = GetSeedPackCount();
                for (int i = 0; i < count; i++)
                {
                    var seed = GetSeedPackAt(i);
                    var uiPreset = GetUIPreset();
                    Blueprint conveyorBlueprint = GetBlueprintUIAt(i);

                    if (!conveyorBlueprint)
                    {
                        conveyorBlueprint = uiPreset.ConveyBlueprint();
                        uiPreset.InsertConveyorBlueprint(i, conveyorBlueprint);
                    }
                    BlueprintViewData viewData = controller.Resources.GetBlueprintViewData(seed);
                    viewData.cost = string.Empty;
                    conveyorBlueprint.UpdateView(viewData);
                }
            }
            #endregion

            #region 添加UI
            public void AddBlueprint(int index)
            {
                var seed = GetSeedPackAt(index);
                var uiPreset = GetUIPreset();
                Blueprint conveyorBlueprint = uiPreset.ConveyBlueprint();
                uiPreset.InsertConveyorBlueprint(index, conveyorBlueprint);
                BlueprintViewData viewData = controller.Resources.GetBlueprintViewData(seed);
                viewData.cost = string.Empty;
                conveyorBlueprint.UpdateView(viewData);
            }
            #endregion

            #region 移除UI
            public void RemoveBlueprintAt(int index)
            {
                var uiPreset = GetUIPreset();
                uiPreset.DestroyConveyorBlueprintAt(index);
            }
            #endregion

            #region 更新UI
            protected override void UpdateBlueprintUI(int index, Blueprint blueprint, SeedPack seed)
            {
                var uiPreset = GetUIPreset();
                blueprint.SetRecharge(0);
                blueprint.SetDisabled(false);
                blueprint.SetTwinkling(seed.IsTwinkling() || (level.IsHoldingTrigger() && seed.CanInstantTrigger()));
                blueprint.SetSelected(level.IsHoldingConveyorBlueprint(index));
                uiPreset.SetConveyorBlueprintNormalizedPosition(index, controller.GetConveyorBlueprintPosition(index));
            }
            #endregion

            #region 操作
            public override void ClickBlueprint(int index)
            {
                if (index < 0 || index >= GetSeedPackCount())
                    return;
                SelectBlueprint(index);
            }
            protected override void PickupBlueprint(int index, bool instantTrigger, bool skipCancelHeld = false)
            {
                // 先取消已经手持的物品。
                if (!skipCancelHeld)
                {
                    if (controller.TryCancelHeldItem())
                        return;
                }
                // 设置当前手持物品。
                level.SetHeldItem(new HeldItemStruct()
                {
                    Type = BuiltinHeldTypes.conveyor,
                    ID = index,
                    InstantTrigger = instantTrigger,
                    Priority = 0,
                });
                level.PlaySound(VanillaSoundID.pick);
            }
            #endregion
        }
        #endregion
    }
}
