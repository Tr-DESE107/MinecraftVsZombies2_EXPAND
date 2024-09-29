using System;
using MVZ2.Extensions;
using MVZ2.GameContent;
using MVZ2.Level.UI;
using MVZ2.Localization;
using MVZ2.UI;
using PVZEngine.Definitions;
using PVZEngine.Level;
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
            if (!CanPickBlueprint(seedPack, out var errorMessage))
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
            var count = level.GetSeedPackCount();
            var levelUI = GetUIPreset();
            levelUI.SetBlueprintCount(count);
        }
        private void UpdateBlueprint(int index)
        {
            var seed = level.GetSeedPackAt(index);
            BlueprintViewData viewData = GetSeedPackViewData(seed);
            var levelUI = GetUIPreset();
            levelUI.SetBlueprintAt(index, viewData);
        }
        private void UpdateBlueprintsView()
        {
            var count = level.GetSeedPackCount();
            var levelUI = GetUIPreset();
            levelUI.SetBlueprintCount(count);
            for (int i = 0; i < count; i++)
            {
                var seed = level.GetSeedPackAt(i);
                BlueprintViewData viewData = GetSeedPackViewData(seed);
                levelUI.SetBlueprintAt(i, viewData);
            }
        }
        private void UpdateBlueprintsState()
        {
            UpdateBlueprintRecharges();
            UpdateBlueprintDisabled();
            UpdateBlueprintSelected();
            UpdateBlueprintTwinkle();
        }
        private BlueprintViewData GetSeedPackViewData(SeedPack seed)
        {
            BlueprintViewData viewData;
            if (seed == null)
            {
                viewData = new BlueprintViewData()
                {
                    empty = true,
                };
            }
            else
            {
                var seedDef = seed.Definition;
                var sprite = GetBlueprintIcon(seedDef);
                viewData = new BlueprintViewData()
                {
                    icon = sprite,
                    cost = seed.GetCost().ToString(),
                    triggerActive = seedDef.IsTriggerActive(),
                    triggerCost = seedDef.GetTriggerCost().ToString(),
                };
            }
            return viewData;
        }
        private void UpdateBlueprintRecharges()
        {
            var levelUI = GetUIPreset();
            var seeds = level.GetAllSeedPacks();
            for (int i = 0; i < seeds.Length; i++)
            {
                var seed = seeds[i];
                if (seed == null)
                    continue;
                var maxCharge = seed.GetMaxRecharge();
                levelUI.SetBlueprintRecharge(i, maxCharge == 0 ? 0 : 1 - seed.GetRecharge() / maxCharge);
            }
        }
        private void UpdateBlueprintDisabled()
        {
            var levelUI = GetUIPreset();
            var seeds = level.GetAllSeedPacks();
            for (int i = 0; i < seeds.Length; i++)
            {
                var seed = seeds[i];
                if (seed == null)
                    continue;
                levelUI.SetBlueprintDisabled(i, !CanPickBlueprint(seed));
            }
        }
        private void UpdateBlueprintSelected()
        {
            var levelUI = GetUIPreset();
            var seeds = level.GetAllSeedPacks();
            for (int i = 0; i < seeds.Length; i++)
            {
                var seed = seeds[i];
                if (seed == null)
                    continue;
                levelUI.SetBlueprintSelected(i, level.IsHoldingBlueprint(i));
            }
        }
        private void UpdateBlueprintTwinkle()
        {
            var levelUI = GetUIPreset();
            var seeds = level.GetAllSeedPacks();
            for (int i = 0; i < seeds.Length; i++)
            {
                var seed = seeds[i];
                if (seed == null)
                    continue;
                levelUI.SetBlueprintTwinkle(i, seed.IsTwinkling());
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
                errorMessage = StringTable.TOOLTIP_RECHARGING;
                return false;
            }
            if (level.Energy < seed.GetCost())
            {
                errorMessage = StringTable.TOOLTIP_NOT_ENOUGH_ENERGY;
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
            var seedDef = seed.Definition;
            return GetBlueprintIcon(seedDef);
        }
        private Sprite GetBlueprintIcon(SeedDefinition seedDef)
        {
            if (seedDef == null)
                return null;
            if (seedDef.GetSeedType() == SeedTypes.ENTITY)
            {
                var entityID = seedDef.GetSeedEntityID();
                if (Main.IsMobile())
                {
                    return Main.ResourceManager.GetSprite(entityID.spacename, $"mobile_blueprint/{entityID.path}");
                }
            }
            return GetHeldItemIcon(seedDef);
        }
        private void ClickBlueprint(int index)
        {
            if (level.IsHoldingItem())
            {
                if (level.CancelHeldItem())
                {
                    level.PlaySound(SoundID.tap);
                }
                return;
            }
            if (!CanPickBlueprint(level.GetSeedPackAt(index)))
            {
                level.PlaySound(SoundID.buzzer);
                return;
            }
            level.PlaySound(SoundID.pick);
            SelectBlueprint(index);
        }
        private void SelectBlueprint(int index)
        {
            level.SetHeldItem(HeldTypes.blueprint, index, 0);
        }

        #endregion
    }
}
