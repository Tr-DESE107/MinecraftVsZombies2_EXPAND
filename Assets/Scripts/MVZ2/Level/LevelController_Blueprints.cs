using System;
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

namespace MVZ2.Level
{
    public partial class LevelController : MonoBehaviour, IDisposable
    {
        #region 私有方法

        private void Awake_Blueprints()
        {
            var levelUI = GetUIPreset();
            levelUI.OnBlueprintPointerEnter += UI_OnBlueprintPointerEnterCallback;
            levelUI.OnBlueprintPointerExit += UI_OnBlueprintPointerExitCallback;
            levelUI.OnBlueprintPointerDown += UI_OnBlueprintPointerDownCallback;
        }

        #region 事件回调

        private void Engine_OnSeedPackChangedCallback(int index)
        {
            UpdateBlueprint(index);
        }
        private void Engine_OnSeedPackCountChangedCallback(int count)
        {
            UpdateBlueprintCount();
        }


        private void UI_OnBlueprintPointerEnterCallback(int index, PointerEventData eventData)
        {
            var levelUI = GetUIPreset();
            string name;
            string tooltip;
            string error = null;
            if (IsGameStarted())
            {
                var seedPack = level.GetSeedPackAt(index);
                if (seedPack == null)
                    return;
                GetBlueprintTooltip(seedPack.Definition, out name, out tooltip);
                if (!CanPickBlueprint(seedPack, out var errorMessage) && !string.IsNullOrEmpty(errorMessage))
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
            levelUI.ShowTooltipOnBlueprint(index, viewData);
        }
        private void UI_OnBlueprintPointerExitCallback(int index, PointerEventData eventData)
        {
            var levelUI = GetUIPreset();
            levelUI.HideTooltip();
        }
        private void UI_OnBlueprintPointerDownCallback(int index, PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;
            ClickBlueprint(index);
        }
        #endregion

        private void UpdateBlueprintCount()
        {
            var count = level.GetSeedSlotCount();
            var levelUI = GetUIPreset();
            levelUI.SetBlueprintSlotCount(count);
        }
        private void UpdateBlueprint(int index)
        {
            var seed = level.GetSeedPackAt(index);
            var uiPreset = GetUIPreset();
            var blueprint = uiPreset.GetBlueprintAt(index);
            if (!blueprint)
            {
                blueprint = uiPreset.CreateBlueprint();
                uiPreset.InsertBlueprint(index, blueprint);
                uiPreset.ForceAlignBlueprint(index);
            }
            BlueprintViewData viewData = Resources.GetBlueprintViewData(seed);
            blueprint.UpdateView(viewData);
        }
        private void UpdateBlueprintsView()
        {
            var count = level.GetSeedSlotCount();
            var levelUI = GetUIPreset();
            levelUI.SetBlueprintSlotCount(count);
            for (int i = 0; i < count; i++)
            {
                UpdateBlueprint(i);
            }
        }
        private void UpdateBlueprintsState()
        {
            var levelUI = GetUIPreset();
            var seeds = level.GetAllSeedPacks();
            for (int i = 0; i < seeds.Length; i++)
            {
                var seed = seeds[i];
                if (seed == null)
                    continue;
                var blueprint = levelUI.GetBlueprintAt(i);
                if (!blueprint)
                    continue;
                var maxCharge = seed.GetMaxRecharge();
                blueprint.SetRecharge(maxCharge == 0 ? 0 : 1 - seed.GetRecharge() / maxCharge);
                blueprint.SetDisabled(!CanPickBlueprint(seed));
                blueprint.SetSelected(level.IsHoldingBlueprint(i));
                blueprint.SetTwinkling(seed.IsTwinkling() || (level.IsHoldingTrigger() && seed.CanInstantTrigger()));
            }
        }
        private bool CanPickBlueprint(SeedPack seed)
        {
            return CanPickBlueprint(seed, out _);
        }
        private bool CanPickBlueprint(SeedPack seed, out string errorMessage)
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
        private void ClickBlueprint(int index)
        {
            if (IsChoosingBlueprints())
            {
                UnchooseBlueprint(index);
                return;
            }

            if (index < 0 || index >= level.GetSeedPackCount())
                return;
            SelectBlueprint(index);
        }
        private void ReleaseOnBlueprint(int index)
        {
            // 移动端会额外在手指放开在蓝图上时进行一次立即触发检测。
            if (!Global.IsMobile())
                return;
            bool holdingTrigger = level.IsHoldingTrigger();
            bool canInstantTrigger = level.GetSeedPackAt(index)?.CanInstantTrigger() ?? false;
            bool usingTrigger = holdingTrigger && canInstantTrigger;
            if (!usingTrigger)
                return;
            bool swapped = IsTriggerSwapped();
            bool instantTrigger = canInstantTrigger && (holdingTrigger != swapped);
            PickupBlueprint(index, instantTrigger, true);
        }
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
        private void SelectBlueprint(int index)
        {
            // 进行立即触发检测。
            bool holdingTrigger = level.IsHoldingTrigger();
            bool canInstantTrigger = level.GetSeedPackAt(index)?.CanInstantTrigger() ?? false;
            bool usingTrigger = holdingTrigger && canInstantTrigger;

            bool swapped = IsTriggerSwapped();
            bool instantTrigger = canInstantTrigger && (holdingTrigger != swapped);
            PickupBlueprint(index, instantTrigger, usingTrigger);
        }
        private void PickupBlueprint(int index, bool instantTrigger, bool skipCancelHeld = false)
        {
            // 先取消已经手持的物品。
            if (!skipCancelHeld)
            {
                if (TryCancelHeldItem())
                    return;
            }
            // 无法拾取蓝图。
            if (!CanPickBlueprint(level.GetSeedPackAt(index)))
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
        private bool IsTriggerSwapped()
        {
            return Main.SaveManager.IsUnlocked(VanillaUnlockID.trigger) && Main.OptionsManager.IsTriggerSwapped();
        }

        #endregion
    }
}
