using System;
using MVZ2.Level.UI;
using MVZ2.UI;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.HeldItems;
using MVZ2.Vanilla.Level;
using MVZ2.Vanilla.SeedPacks;
using MVZ2Logic.Level;
using MVZ2Logic.SeedPacks;
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
            var seedPack = level.GetSeedPackAt(index);
            if (seedPack == null)
                return;
            var seedDef = seedPack.Definition;
            if (seedDef == null || seedDef.GetSeedType() != SeedTypes.ENTITY)
                return;
            var entityID = seedDef.GetSeedEntityID();
            var name = Main.ResourceManager.GetEntityName(entityID);
            var tooltip = Main.ResourceManager.GetEntityTooltip(entityID);
            string error = null;
            if (!CanPickBlueprint(seedPack, out var errorMessage) && !string.IsNullOrEmpty(errorMessage))
            {
                error = Main.LanguageManager._(errorMessage);
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
            BlueprintViewData viewData = Main.ResourceManager.GetBlueprintViewData(seed);
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
                blueprint.SetTwinkling(seed.IsTwinkling());
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
            return Main.ResourceManager.GetBlueprintIcon(seed.Definition);
        }
        private void ClickBlueprint(int index)
        {
            if (IsChoosingBlueprints())
            {
                var choosingIndex = chosenBlueprints[index];
                chosenBlueprints.RemoveAt(index);

                var uiPreset = GetUIPreset();
                var blueprint = uiPreset.GetBlueprintAt(index);
                uiPreset.RemoveBlueprintAt(index);

                var startPos = blueprint.transform.position;
                var targetBlueprint = uiPreset.GetBlueprintChooseItem(choosingIndex);
                var movingBlueprint = uiPreset.CreateMovingBlueprint();
                movingBlueprint.transform.position = startPos;
                movingBlueprint.SetBlueprint(blueprint);
                movingBlueprint.SetMotion(startPos, targetBlueprint.transform);
                movingBlueprint.OnMotionFinished += () =>
                {
                    UpdateBlueprintChooseItem(choosingIndex);
                    uiPreset.RemoveMovingBlueprint(movingBlueprint);
                };

                level.PlaySound(VanillaSoundID.tap);
                return;
            }

            if (level.IsHoldingItem())
            {
                if (level.CancelHeldItem())
                {
                    level.PlaySound(VanillaSoundID.tap);
                }
                return;
            }
            if (!CanPickBlueprint(level.GetSeedPackAt(index)))
            {
                level.PlaySound(VanillaSoundID.buzzer);
                return;
            }
            level.PlaySound(VanillaSoundID.pick);
            SelectBlueprint(index);
        }
        private void SelectBlueprint(int index)
        {
            level.SetHeldItem(BuiltinHeldTypes.blueprint, index, 0);
        }

        #endregion
    }
}
