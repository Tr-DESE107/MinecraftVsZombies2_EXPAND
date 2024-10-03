using System;
using System.Collections.Generic;
using MukioI18n;
using MVZ2.Definitions;
using MVZ2.Extensions;
using MVZ2.GameContent;
using MVZ2.Level.UI;
using MVZ2.Localization;
using MVZ2.Resources;
using MVZ2.UI;
using MVZ2.Vanilla;
using PVZEngine;
using PVZEngine.Definitions;
using PVZEngine.Level;
using Tools;
using UnityEngine;
using UnityEngine.EventSystems;
using static MVZ2.Level.UI.LevelUIPreset;

namespace MVZ2.Level
{
    public partial class LevelController : MonoBehaviour, IDisposable
    {
        #region 公有方法

        public void SetHeldItemUI(NamespaceID heldType, long id, int priority, bool noCancel)
        {
            Sprite icon = null;
            if (heldType == HeldTypes.blueprint)
            {
                icon = GetHeldItemIcon(id);
            }
            else if (heldType == HeldTypes.pickaxe)
            {
                icon = Main.LanguageManager.GetSprite(pickaxeSprite);
            }
            else if (heldType == HeldTypes.starshard)
            {
                icon = Main.LanguageManager.GetSprite(SpritePaths.GetStarshardIcon(level.AreaDefinition.GetID()));
            }
            ui.SetHeldItemIcon(icon);


            List<int> layers = new List<int>();
            layers.Add(Layers.RAYCAST_RECEIVER);
            if (level.IsHeldItemForGrid(heldType))
            {
                layers.Add(Layers.GRID);
            }
            if (level.IsHeldItemForEntity(heldType))
            {
                layers.Add(Layers.DEFAULT);
            }
            if (level.IsHeldItemForPickup(heldType))
            {
                layers.Add(Layers.PICKUP);
            }
            LayerMask layerMask = Layers.GetMask(layers.ToArray());
            var uiPreset = GetUIPreset();
            uiPreset.SetRaycasterMask(layerMask);
            raycaster.eventMask = layerMask;
        }
        public void ShowMoney()
        {
            var levelUI = GetUIPreset();
            levelUI.ResetMoneyFadeTime();
        }
        public LevelUIPreset GetUIPreset()
        {
            return Main.IsMobile() ? mobileUI : standaloneUI;
        }
        public void ShowRestartConfirmDialog()
        {
            var title = Main.LanguageManager._(StringTable.RESTART);
            var desc = Main.LanguageManager._(DIALOG_DESC_RESTART);
            Main.Scene.ShowDialogConfirm(title, desc, async (confirm) =>
            {
                if (confirm)
                {
                    await RestartLevel();
                }
            });
        }
        public void ShowLevelErrorLoadingDialog(Exception e)
        {
            ShowLevelErrorLoadingDialog(Main.LanguageManager._(ERROR_LOAD_LEVEL_EXCEPTION, e.Message));
        }

        #endregion

        #region 私有方法

        private void Awake_UI()
        {
            ui.OnPauseDialogResumeClicked += UI_OnPauseDialogResumeClickedCallback;
            ui.OnLevelLoadedDialogButtonClicked += UI_OnLevelLoadedDialogOptionClickedCallback;
            ui.OnLevelErrorLoadingDialogButtonClicked += UI_OnLevelErrorLoadingDialogOptionClickedCallback;

            ui.OnGameOverRetryButtonClicked += UI_OnGameOverRetryButtonClickedCallback;
            ui.OnGameOverBackButtonClicked += UI_OnGameOverBackButtonClickedCallback;

            ui.SetHeldItemIcon(null);
            ui.SetPauseDialogActive(false);
            ui.SetOptionsDialogActive(false);
            ui.SetGameOverDialogActive(false);
            ui.SetLevelLoadedDialogVisible(false);
            ui.SetLevelErrorLoadingDialogVisible(false);

            var uiPreset = GetUIPreset();
            uiPreset.OnPickaxePointerEnter += UI_OnPickaxePointerEnterCallback;
            uiPreset.OnPickaxePointerExit += UI_OnPickaxePointerExitCallback;
            uiPreset.OnPickaxePointerDown += UI_OnPickaxePointerDownCallback;

            uiPreset.OnStarshardPointerDown += UI_OnStarshardPointerDownCallback;

            uiPreset.OnTriggerPointerDown += UI_OnTriggerPointerDownCallback;

            uiPreset.OnRaycastReceiverPointerDown += UI_OnRaycastReceiverPointerDownCallback;
            uiPreset.OnMenuButtonClick += UI_OnMenuButtonClickCallback;
            uiPreset.OnSpeedUpButtonClick += UI_OnSpeedUpButtonClickCallback;

            uiPreset.HideMoney();
            uiPreset.SetProgressVisible(false);
            uiPreset.HideTooltip();
            SetUIVisibleState(VisibleState.Nothing);
        }

        #region 事件回调

        #region UI方
        private void UI_OnRaycastReceiverPointerDownCallback(LevelUIPreset.Receiver receiver, PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            ClickOnReceiver(receiver);
        }
        private void UI_OnPickaxePointerEnterCallback(PointerEventData eventData)
        {
            var levelUI = GetUIPreset();
            var viewData = new TooltipViewData()
            {
                name = Main.LanguageManager._(StringTable.TOOLTIP_DIG_CONTRAPTION),
                error = level.IsPickaxeDisabled() ? Main.LanguageManager._(level.GetPickaxeDisableMessage()) : null,
                description = null
            };
            levelUI.ShowTooltipOnPickaxe(viewData);
        }
        private void UI_OnPickaxePointerExitCallback(PointerEventData eventData)
        {
            var levelUI = GetUIPreset();
            levelUI.HideTooltip();
        }
        private void UI_OnPickaxePointerDownCallback(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;
            ClickPickaxe();
        }
        private void UI_OnMenuButtonClickCallback()
        {
            Pause();
            level.PlaySound(SoundID.pause);
            ShowOptionsDialog();
        }
        private void UI_OnOptionsMenuCloseCallback()
        {
            Resume();
        }
        private void UI_OnSpeedUpButtonClickCallback()
        {
            SwitchSpeedUp();
        }
        private void UI_OnStarshardPointerDownCallback(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;
            ClickStarshard();
        }
        private void UI_OnTriggerPointerDownCallback(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;
        }
        private void UI_OnPauseDialogResumeClickedCallback()
        {
            Resume();
        }
        private async void UI_OnGameOverRetryButtonClickedCallback()
        {
            ui.SetGameOverDialogInteractable(false);
            await RestartLevel();
        }
        private async void UI_OnGameOverBackButtonClickedCallback()
        {
            await ExitLevel();
        }
        private async void UI_OnLevelLoadedDialogOptionClickedCallback(LevelLoadedDialog.ButtonType type)
        {
            switch (type)
            {
                case LevelLoadedDialog.ButtonType.Resume:
                    Resume();
                    ui.SetLevelLoadedDialogVisible(false);
                    levelLoaded = false;
                    break;
                case LevelLoadedDialog.ButtonType.Restart:
                    ShowRestartConfirmDialog();
                    break;
                default:
                    await ExitLevel();
                    break;
            }
        }
        private async void UI_OnLevelErrorLoadingDialogOptionClickedCallback(bool restart)
        {
            ui.SetLevelErrorLoadingDialogInteractable(false);
            if (restart)
            {
                await RestartLevel();
            }
            else
            {
                await ExitLevel();
            }
        }

        #endregion

        #endregion

        #region 对话框
        private void ShowOptionsDialog()
        {
            ui.SetOptionsDialogActive(true);

            optionsLogic = new OptionsLogicLevel(ui.OptionsDialog, this);
            optionsLogic.InitDialog();
            optionsLogic.OnClose += UI_OnOptionsMenuCloseCallback;
        }
        private void ShowPausedDialog()
        {
            var spriteReference = pauseImages.Random(uiRandom);
            ui.SetPauseDialogActive(true);
            ui.SetPauseDialogImage(Main.LanguageManager.GetSprite(spriteReference));
        }
        private void ShowGameOverDialog()
        {
            string message;
            if (killerID != null)
            {
                message = Main.ResourceManager.GetEntityDeathMessage(killerID);
            }
            else
            {
                message = deathMessage;
            }
            ui.SetGameOverDialogActive(true);
            ui.SetGameOverDialogMessage(Main.LanguageManager._p(StringTable.CONTEXT_DEATH_MESSAGE, message));
        }
        private void ShowLevelErrorLoadingDialog(string desc)
        {
            ui.SetLevelErrorLoadingDialogVisible(true);
            ui.SetLevelErrorLoadingDialogDesc(desc);
        }
        private void ShowLevelErrorLoadingDialog()
        {
            ShowLevelErrorLoadingDialog(Main.LanguageManager._(ERROR_LOAD_LEVEL_IDENTIFIER_NOT_MATCH));
        }
        private void ShowLevelLoadedDialog()
        {
            ui.SetLevelLoadedDialogVisible(true);
        }
        #endregion

        #region 手持物品
        private Sprite GetHeldItemIcon(long i)
        {
            var seeds = level.GetAllSeedPacks();
            var seed = seeds[i];
            return GetHeldItemIcon(seed);
        }
        private Sprite GetHeldItemIcon(SeedPack seed)
        {
            if (seed == null)
                return null;
            var seedDef = seed.Definition;
            return GetHeldItemIcon(seedDef);
        }
        private Sprite GetHeldItemIcon(SeedDefinition seedDef)
        {
            if (seedDef == null)
                return null;
            Sprite sprite = null;
            if (seedDef.GetSeedType() == SeedTypes.ENTITY)
            {
                var entityID = seedDef.GetSeedEntityID();
                var modelID = entityID.ToModelID(EngineModelID.TYPE_ENTITY);
                sprite = Main.ResourceManager.GetModelIcon(modelID);
            }
            return sprite;
        }
        #endregion
        private void ClickPickaxe()
        {
            if (!PickaxeActive)
                return;
            if (level.IsHoldingItem())
            {
                if (level.CancelHeldItem())
                {
                    level.PlaySound(SoundID.tap);
                }
                return;
            }
            if (level.IsPickaxeDisabled())
                return;
            level.PlaySound(SoundID.pickaxe);
            level.SetHeldItem(HeldTypes.pickaxe, 0, 0);
        }
        private void ClickStarshard()
        {
            if (!StarshardActive)
                return;
            if (level.IsHoldingItem())
            {
                if (level.CancelHeldItem())
                {
                    level.PlaySound(SoundID.tap);
                }
                return;
            }
            if (level.GetStarshardCount() <= 0)
            {
                level.PlaySound(SoundID.buzzer);
                return;
            }
            level.SetHeldItem(HeldTypes.starshard, 0, 0);
        }
        private void ClickOnReceiver(RaycastReceiver receiver)
        {
            var levelUI = GetUIPreset();
            var type = levelUI.GetReceiverType(receiver);
            ClickOnReceiver(type);
        }
        private void ClickOnReceiver(LevelUIPreset.Receiver receiver)
        {
            if (!IsGameRunning())
                return;

            LawnArea area = LawnArea.Main;
            switch (receiver)
            {
                case LevelUIPreset.Receiver.Side:
                    area = LawnArea.Side;
                    break;
                case LevelUIPreset.Receiver.Bottom:
                    area = LawnArea.Bottom;
                    break;
            }
            level.UseOnLawn(area);
        }
        private void AdvanceLevelProgress()
        {
            var deltaTime = Time.deltaTime;
            var totalFlags = level.GetTotalFlags();
            if (bannerProgresses == null || bannerProgresses.Length != totalFlags)
            {
                var newProgresses = new float[totalFlags];
                if (bannerProgresses != null)
                {
                    bannerProgresses.CopyTo(newProgresses, 0);
                }
                bannerProgresses = newProgresses;
            }
            for (int i = 0; i < bannerProgresses.Length; i++)
            {
                float value = level.CurrentWave >= i * level.GetWavesPerFlag() ? deltaTime : -deltaTime;
                bannerProgresses[i] = Mathf.Clamp01(bannerProgresses[i] + value);
            }
            int totalWaveCount = level.GetTotalWaveCount();
            float targetProgress = totalWaveCount <= 0 ? 0 : level.CurrentWave / (float)totalWaveCount;
            int progressDirection = Math.Sign(targetProgress - levelProgress);
            if (progressDirection != 0)
            {
                levelProgress += Time.deltaTime * 0.1f * progressDirection;
                var newDirection = Mathf.Sign(targetProgress - levelProgress);
                if (progressDirection != newDirection)
                {
                    levelProgress = targetProgress;
                }
            }
        }
        private void PlayReadySetBuild()
        {
            var ui = GetUIPreset();
            ui.ShowReadySetBuild();
        }
        private void UpdateLevelUI()
        {
            var ui = GetUIPreset();
            ui.SetEnergy(Mathf.FloorToInt(Mathf.Max(0, level.Energy - level.GetDelayedEnergy())).ToString());
            ui.SetMoney((level.GetMoney() - level.GetDelayedMoney()).ToString("N0"));
            ui.SetPickaxeVisible(!level.IsHoldingPickaxe());
            UpdateLevelProgress();
            UpdateBlueprintsState();
            UpdateStarshards();
        }
        private void SetLevelUISimulationSpeed(float simulationSpeed)
        {
            var ui = GetUIPreset();
            ui.SetSimulationSpeed(simulationSpeed);
        }
        private void UpdateLevelProgress()
        {
            var ui = GetUIPreset();
            ui.SetProgressVisible(level.LevelProgressVisible);
            ui.SetProgress(levelProgress);
            ui.SetBannerProgresses(bannerProgresses);
        }
        private void UpdateLevelName()
        {
            string name = level.GetLevelName();
            int dayNumber = level.GetDayNumber();
            if (string.IsNullOrEmpty(name))
            {
                name = StringTable.LEVEL_NAME_UNKNOWN;
            }
            var levelUI = GetUIPreset();
            var levelName = Main.LanguageManager._p(StringTable.CONTEXT_LEVEL_NAME, name);
            if (dayNumber > 0)
            {
                levelName = Main.LanguageManager._p(StringTable.CONTEXT_LEVEL_NAME, StringTable.LEVEL_NAME_DAY_TEMPLATE, levelName, dayNumber);
            }
            levelUI.SetLevelName(levelName);
        }
        private void UpdateStarshards()
        {
            var levelUI = GetUIPreset();
            levelUI.SetStarshardCount(level.GetStarshardCount(), 3);
        }
        private void UpdateDifficultyName()
        {
            var difficultyName = Main.ResourceManager.GetDifficultyName(level.Difficulty);
            var levelUI = GetUIPreset();
            levelUI.SetDifficulty(difficultyName);
        }
        private void SetUIVisibleState(VisibleState state)
        {
            var levelUI = GetUIPreset();
            levelUI.SetUIVisibleState(state);
        }
        private void SetUnlockedUIVisible()
        {
            var levelUI = GetUIPreset();
            StarshardActive = Main.SaveManager.IsStarshardUnlocked();
            TriggerActive = Main.SaveManager.IsTriggerUnlocked();
        }

        #endregion

        #region 属性字段

        [TranslateMsg("对话框内容")]
        public const string DIALOG_DESC_RESTART = "确认要重新开始关卡吗？\n本关的进度都将丢失。";
        [TranslateMsg("对话框内容，{0}为错误信息")]
        public const string ERROR_LOAD_LEVEL_EXCEPTION = "加载关卡失败，出现错误：{0}";
        [TranslateMsg("对话框内容")]
        public const string ERROR_LOAD_LEVEL_IDENTIFIER_NOT_MATCH = "加载关卡失败，存档状态和当前游戏状态不匹配。";

        #region 保存属性
        private RandomGenerator uiRandom = new RandomGenerator(Guid.NewGuid().GetHashCode());
        private float levelProgress;
        private float[] bannerProgresses;
        #endregion

        private OptionsLogicLevel optionsLogic;

        [Header("UI")]
        [SerializeField]
        private Physics2DRaycaster raycaster;
        [SerializeField]
        private LevelUIPreset standaloneUI;
        [SerializeField]
        private LevelUIPreset mobileUI;
        [SerializeField]
        private List<Sprite> pauseImages = new List<Sprite>();
        [SerializeField]
        private Sprite pickaxeSprite;
        #endregion
    }
}
