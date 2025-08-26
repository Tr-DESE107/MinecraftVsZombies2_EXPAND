﻿using MVZ2.Level.UI;
using MVZ2.Managers;
using MVZ2.Options;
using MVZ2.UI;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Saves;
using MVZ2Logic;
using MVZ2Logic.Games;
using MVZ2Logic.HeldItems;
using MVZ2Logic.Level;
using PVZEngine;
using PVZEngine.Level;
using UnityEngine;
using UnityEngine.EventSystems;
using static MVZ2.Level.UI.LevelUIPreset;

namespace MVZ2.Level
{
    public partial class LevelController
    {
        private void Awake_UI()
        {
            ui.OnExitLevelToNoteCalled += OnUIExitLevelToNoteCalledCallback;
            ui.OnStartGameCalled += StartGame;
            ui.SetMobile(Main.IsMobile());

            var uiPreset = GetUIPreset();
            uiPreset.OnRaycastReceiverPointerInteraction += UI_OnRaycastReceiverPointerInteractionCallback;
            uiPreset.OnMenuButtonClick += UI_OnMenuButtonClickCallback;
            uiPreset.OnSpeedUpButtonClick += UI_OnSpeedUpButtonClickCallback;

            uiPreset.HideMoney();
            SetUIVisibleState(VisibleState.Nothing);
        }
        private void InitLevelEngine_UI(LevelEngine level)
        {
            levelRaycaster.Init(level);
        }
        private void StartGame_UI()
        {
            // 设置UI可见状态
            SetUIVisibleState(VisibleState.InLevel);
            RefreshUIAtLevelStart();

            var uiPreset = GetUIPreset();
            uiPreset.SetReceiveRaycasts(true);
            uiPreset.UpdateFrame(0);
        }
        private void WriteToSerializable_UI(SerializableLevelController seri)
        {
            seri.uiPreset = GetUIPreset().ToSerializable();
        }
        private void ReadFromSerializable_UI(SerializableLevelController seri)
        {
            // uiPreset的animator.Update会导致第一次加载该场景时，蓝图UI的子模型显示状态不正确，所以放在前面
            var uiPreset = GetUIPreset();
            uiPreset.LoadFromSerializable(seri.uiPreset);
            uiPreset.UpdateFrame(0);
        }
        public ILevelUI GetUI()
        {
            return ui;
        }
        public LevelUIPreset GetUIPreset()
        {
            return ui.GetUIPreset();
        }
        private void RefreshUIAtLevelInit()
        {
            var uiPreset = GetUIPreset();
            uiPreset.UpdateFrame(0);
            SetStarshardIcon();
            UpdateHotkeyTexts();
        }
        private void RefreshUIAtLevelStart()
        {
            // 关卡名
            UpdateLevelName();
            // 能量、关卡进度条、手持物品、蓝图状态、星之碎片
            UpdateInLevelUI(0);
            // 金钱
            UpdateMoney();
            // 难度名称
            UpdateDifficultyName();
            // 关卡进度条
            RefreshProgressBar();
        }

        #region 金钱
        public void ShowMoney()
        {
            var levelUI = GetUIPreset();
            levelUI.ResetMoneyFadeTime();
        }
        public void SetMoneyFade(bool fade)
        {
            var levelUI = GetUIPreset();
            levelUI.SetMoneyFade(fade);
        }
        private void UpdateMoney()
        {
            var ui = GetUIPreset();
            ui.SetMoney((level.GetMoney() - level.GetDelayedMoney()).ToString("N0"));
        }
        #endregion

        #region 关卡名
        public void UpdateLevelName()
        {
            var levelUI = GetUIPreset();
            levelUI.SetLevelName(LevelManager.GetStageName(level));
        }
        #endregion

        #region 难度
        private void UpdateDifficultyName()
        {
            var difficultyName = Game.GetDifficultyName(level.Difficulty);
            var levelUI = GetUIPreset();
            levelUI.SetDifficulty(difficultyName);
        }
        #endregion

        #region 提示红字
        private void PlayReadySetBuild()
        {
            var ui = GetUIPreset();
            ui.ShowReadySetBuild();
            level.PlaySound(VanillaSoundID.readySetBuild);
        }
        #endregion

        #region 能量
        public void FlickerEnergy()
        {
            var levelUI = GetUIPreset();
            levelUI.FlickerEnergy();
        }
        private void UpdateEnergy()
        {
            var ui = GetUIPreset();
            ui.SetEnergy(Mathf.FloorToInt(Mathf.Max(0, level.Energy - level.GetDelayedEnergy())).ToString());
        }
        #endregion

        #region UI可见度
        private void SetUIVisibleState(VisibleState state)
        {
            var levelUI = GetUIPreset();
            levelUI.SetUIVisibleState(state);
        }
        #endregion

        #region 热键
        public void UpdateHotkeyTexts()
        {
            var preset = ui.GetUIPreset();
            preset.SetPickaxeHotkeyText(GetHotkeyName(HotKeys.pickaxe));
            preset.SetStarshardHotkeyText(GetHotkeyName(HotKeys.starshard));
            preset.SetTriggerHotkeyText(GetHotkeyName(HotKeys.trigger));
            preset.SetSpeedUpHotkeyText(GetHotkeyName(HotKeys.fastForward));
            blueprintController.ForceUpdateBlueprintHotkeyTexts();
        }
        private string GetHotkeyName(NamespaceID keyID)
        {
            if (Global.Game.IsMobile() || !Main.OptionsManager.ShowHotkeyIndicators())
                return string.Empty;
            var keycode = Main.OptionsManager.GetKeyBinding(keyID);
            return keycode != KeyCode.None ? Main.InputManager.GetKeyCodeName(keycode) : string.Empty;
        }
        #endregion

        public void SetUIAndInputDisabled(bool disabled)
        {
            inputAndUIDisabled = disabled;
            ui.SetUIDisabled(disabled);
        }
        /// <summary>
        /// 更新能量、关卡进度条、手持物品、蓝图状态、星之碎片。
        /// </summary>
        private void UpdateInLevelUI(float deltaTime)
        {
            var ui = GetUIPreset();
            UpdateEnergy();
            UpdateLevelProgressUI();
            UpdateHeldSlotUI();
            UpdateStarshards();
        }

        #region 事件回调
        private void UI_OnRaycastReceiverPointerInteractionCallback(LawnArea area, PointerEventData eventData, PointerInteraction interaction)
        {
            if (!IsGameRunning())
                return;
            var target = new HeldItemTargetLawn(level, area);
            var pointerParams = InputManager.GetPointerInteractionParamsFromEventData(eventData, interaction);
            level.DoHeldItemPointerEvent(target, pointerParams);
        }
        private void UI_OnMenuButtonClickCallback()
        {
            if (IsGameRunning())
            {
                if (!IsPauseDisabled())
                {
                    PauseGame();
                    level.PlaySound(VanillaSoundID.pause);
                    ShowOptionsDialog();
                }
            }
            else
            {
                ShowOptionsDialog();
            }
        }
        private void UI_OnOptionsMenuCloseCallback()
        {
            if (!IsGameStarted())
            {
                if (optionsLogic != null)
                {
                    optionsLogic.Dispose();
                    optionsLogic = null;
                }
                ui.SetOptionsDialogActive(false);
            }
            else
            {
                ResumeGameDelayed(100);
            }
        }
        private void UI_OnSpeedUpButtonClickCallback()
        {
            SwitchSpeedUp();
        }
        private void UI_OnBlueprintPointerInteractionCallback(int index, PointerEventData eventData, PointerInteraction interaction, bool conveyor)
        {
            if (!IsGameRunning())
                return;
            var target = new HeldItemTargetBlueprint(level, index, conveyor);
            var pointerParams = InputManager.GetPointerInteractionParamsFromEventData(eventData, interaction);
            level.DoHeldItemPointerEvent(target, pointerParams);
        }

        #endregion

        #region 属性字段
        private bool inputAndUIDisabled;

        [Header("UI")]
        [SerializeField]
        private LevelUI ui;
        [SerializeField]
        private LevelRaycaster levelRaycaster;
        #endregion
    }
}
