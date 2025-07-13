using MVZ2.GameContent.HeldItems;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Level;
using MVZ2.Vanilla.Saves;
using MVZ2Logic.Level;
using UnityEngine.EventSystems;

namespace MVZ2.Level
{
    public partial class LevelController
    {
        private void Awake_Tools()
        {
            var uiPreset = GetUIPreset();
            uiPreset.OnPickaxePointerEnter += UI_OnPickaxePointerEnterCallback;
            uiPreset.OnPickaxePointerExit += UI_OnPickaxePointerExitCallback;
            uiPreset.OnPickaxePointerDown += UI_OnPickaxePointerDownCallback;

            uiPreset.OnStarshardPointerDown += UI_OnStarshardPointerDownCallback;

            uiPreset.OnTriggerPointerEnter += UI_OnTriggerPointerEnterCallback;
            uiPreset.OnTriggerPointerExit += UI_OnTriggerPointerExitCallback;
            uiPreset.OnTriggerPointerDown += UI_OnTriggerPointerDownCallback;
        }
        private void StartGame_Tools()
        {
            // 可解锁UI
            UpdateToolUIUnlockedActive();
        }
        private void WriteToSerializable_Tools(SerializableLevelController seri)
        {
            seri.energyActive = EnergyActive;
            seri.blueprintsActive = BlueprintsActive;
            seri.pickaxeActive = PickaxeActive;
            seri.starshardActive = StarshardActive;
            seri.triggerActive = TriggerActive;
        }
        private void ReadFromSerializable_Tools(SerializableLevelController seri)
        {
            EnergyActive = seri.energyActive;
            BlueprintsActive = seri.blueprintsActive;
            PickaxeActive = seri.pickaxeActive;
            StarshardActive = seri.starshardActive;
            TriggerActive = seri.triggerActive;
        }
        private void UpdateToolUIUnlockedActive()
        {
            var levelUI = GetUIPreset();
            StarshardActive = Saves.IsStarshardUnlocked();
            TriggerActive = Saves.IsTriggerUnlocked();
        }

        #region 铁镐
        private void ClickPickaxe()
        {
            if (!PickaxeActive)
                return;
            if (level.IsHoldingExclusiveItem())
            {
                if (level.CancelHeldItem())
                {
                    level.PlaySound(VanillaSoundID.tap);
                }
                return;
            }
            if (!level.CanUsePickaxe())
                return;
            level.PlaySound(VanillaSoundID.pickaxe);
            level.SetHeldItem(VanillaHeldTypes.pickaxe, 0, 0);
        }
        private void UI_OnPickaxePointerEnterCallback(PointerEventData eventData)
        {
            if (!IsGameStarted())
                return;
            ShowTooltip(pickaxeTooltipSource);
        }
        private void UI_OnPickaxePointerExitCallback(PointerEventData eventData)
        {
            HideTooltip();
        }
        private void UI_OnPickaxePointerDownCallback(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;
            if (!IsGameStarted())
                return;
            ClickPickaxe();
        }
        #endregion

        #region 星之碎片
        private void ClickStarshard()
        {
            if (!StarshardActive)
                return;
            if (level.IsHoldingExclusiveItem())
            {
                if (level.CancelHeldItem())
                {
                    level.PlaySound(VanillaSoundID.tap);
                }
                return;
            }
            if (!level.CanUseStarshard())
            {
                level.PlaySound(VanillaSoundID.buzzer);
                return;
            }
            level.SetHeldItem(VanillaHeldTypes.starshard, 0, 0);
        }
        private void UpdateStarshards()
        {
            var levelUI = GetUIPreset();
            levelUI.SetStarshardCount(level.GetStarshardCount(), level.GetStarshardSlotCount());
        }
        private void SetStarshardIcon()
        {
            var levelUI = GetUIPreset();
            var spriteRef = level.GetStarshardIcon();
            var sprite = Main.GetFinalSprite(spriteRef);
            levelUI.SetStarshardIcon(sprite);
        }
        private void UI_OnStarshardPointerDownCallback(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;
            if (!IsGameStarted())
                return;
            ClickStarshard();
        }
        #endregion

        #region 触发器
        private void ClickTrigger()
        {
            if (!TriggerActive)
                return;
            if (level.IsHoldingExclusiveItem())
            {
                if (level.CancelHeldItem())
                {
                    level.PlaySound(VanillaSoundID.tap);
                }
                return;
            }
            if (!level.CanUseTrigger())
                return;
            level.SetHeldItem(VanillaHeldTypes.trigger, 0, 0);
        }
        private void UI_OnTriggerPointerEnterCallback(PointerEventData eventData)
        {
            if (!IsGameStarted())
                return;
            ShowTooltip(triggerTooltipSource);
        }
        private void UI_OnTriggerPointerExitCallback(PointerEventData eventData)
        {
            HideTooltip();
        }
        private void UI_OnTriggerPointerDownCallback(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;
            if (!IsGameStarted())
                return;
            ClickTrigger();
        }
        #endregion

        #region 属性字段
        public bool EnergyActive
        {
            get => energyActive;
            set
            {
                energyActive = value;
                var uiPreset = GetUIPreset();
                uiPreset.SetEnergyActive(value);
            }
        }
        public bool BlueprintsActive
        {
            get => blueprintsActive;
            set
            {
                blueprintsActive = value;
                ui.Blueprints.SetBlueprintsActive(value);
            }
        }
        public bool PickaxeActive
        {
            get => pickaxeActive;
            set
            {
                pickaxeActive = value;
                var uiPreset = GetUIPreset();
                uiPreset.SetPickaxeActive(value);
            }
        }
        public bool StarshardActive
        {
            get => starshardActive;
            set
            {
                starshardActive = value;
                var uiPreset = GetUIPreset();
                uiPreset.SetStarshardActive(value);
            }
        }
        public bool TriggerActive
        {
            get => triggerActive;
            set
            {
                triggerActive = value;
                var uiPreset = GetUIPreset();
                uiPreset.SetTriggerActive(value);
            }
        }
        private bool energyActive = true;
        private bool blueprintsActive = true;
        private bool pickaxeActive = true;
        private bool starshardActive = true;
        private bool triggerActive = true;
        #endregion
    }
}
