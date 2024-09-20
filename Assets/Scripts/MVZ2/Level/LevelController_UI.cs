using System;
using System.Collections.Generic;
using MukioI18n;
using MVZ2.GameContent;
using MVZ2.Level.UI;
using MVZ2.Vanilla;
using PVZEngine;
using PVZEngine.Definitions;
using PVZEngine.Level;
using Tools;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MVZ2.Level
{
    public partial class LevelController : MonoBehaviour, IDisposable
    {
        #region 公有方法

        public void SetHeldItemUI(NamespaceID heldType, long id, int priority, bool noCancel)
        {
            var ui = GetLevelUI();
            Sprite icon = null;
            LayerMask layerMask = Layers.GetMask(Layers.DEFAULT, Layers.PICKUP);
            if (heldType == HeldTypes.blueprint)
            {
                icon = GetHeldItemIcon(id);
                layerMask = Layers.GetMask(Layers.GRID, Layers.RAYCAST_RECEIVER);
            }
            else if (heldType == HeldTypes.pickaxe)
            {
                icon = Main.LanguageManager.GetSprite(SpritePaths.pickaxe);
                layerMask = Layers.GetMask(Layers.DEFAULT, Layers.RAYCAST_RECEIVER);
            }
            else if (heldType == HeldTypes.starshard)
            {
                icon = Main.LanguageManager.GetSprite(SpritePaths.GetStarshardIcon(level.AreaDefinition.GetID()));
                layerMask = Layers.GetMask(Layers.DEFAULT, Layers.RAYCAST_RECEIVER);
            }
            ui.SetHeldItemIcon(icon);
            ui.SetRaycasterMask(layerMask);
            raycaster.eventMask = layerMask;
        }
        public LevelUI GetLevelUI()
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
            var levelUI = GetLevelUI();
            levelUI.OnPickaxePointerEnter += UI_OnPickaxePointerEnterCallback;
            levelUI.OnPickaxePointerExit += UI_OnPickaxePointerExitCallback;
            levelUI.OnPickaxePointerDown += UI_OnPickaxePointerDownCallback;
            levelUI.OnRaycastReceiverPointerDown += UI_OnRaycastReceiverPointerDownCallback;
            levelUI.OnMenuButtonClick += UI_OnMenuButtonClickCallback;
            levelUI.OnSpeedUpButtonClick += UI_OnSpeedUpButtonClickCallback;
            levelUI.OnStarshardPointerDown += UI_OnStarshardPointerDownCallback;
            levelUI.OnPauseDialogResumeClicked += UI_OnPauseDialogResumeClickedCallback;
            levelUI.OnLevelLoadedDialogButtonClicked += UI_OnLevelLoadedDialogOptionClickedCallback;
            levelUI.OnLevelErrorLoadingDialogButtonClicked += UI_OnLevelErrorLoadingDialogOptionClickedCallback;

            levelUI.OnGameOverRetryButtonClicked += UI_OnGameOverRetryButtonClickedCallback;
            levelUI.OnGameOverBackButtonClicked += UI_OnGameOverBackButtonClickedCallback;

            levelUI.SetHeldItemIcon(null);
            levelUI.HideMoney();
            levelUI.SetProgressVisible(false);
            levelUI.SetHugeWaveTextVisible(false);
            levelUI.SetFinalWaveTextVisible(false);
            levelUI.SetReadySetBuildVisible(false);
            levelUI.SetPauseDialogActive(false);
            levelUI.SetOptionsDialogActive(false);
            levelUI.SetGameOverDialogActive(false);
            levelUI.SetLevelLoadedDialogVisible(false);
            levelUI.SetLevelErrorLoadingDialogVisible(false);
            levelUI.SetYouDiedVisible(false);
            levelUI.HideTooltip();
            SetUIVisibleState(VisibleState.Nothing);
        }

        #region 事件回调

        #region UI方
        private void UI_OnRaycastReceiverPointerDownCallback(LevelUI.Receiver receiver, PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            if (!IsGameRunning())
                return;

            LawnArea area = LawnArea.Main;
            switch (receiver)
            {
                case LevelUI.Receiver.Side:
                    area = LawnArea.Side;
                    break;
                case LevelUI.Receiver.Bottom:
                    area = LawnArea.Bottom;
                    break;
            }
            level.UseOnLawn(area);
        }
        private void UI_OnPickaxePointerEnterCallback(PointerEventData eventData)
        {
            var levelUI = GetLevelUI();
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
            var levelUI = GetLevelUI();
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
        private void UI_OnPauseDialogResumeClickedCallback()
        {
            Resume();
        }
        private async void UI_OnGameOverRetryButtonClickedCallback()
        {
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
                    var ui = GetLevelUI();
                    ui.SetLevelLoadedDialogVisible(false);
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
            var levelUI = GetLevelUI();
            levelUI.SetLevelErrorLoadingDialogInteractable(false);
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
            var levelUI = GetLevelUI();
            levelUI.SetOptionsDialogActive(true);

            optionsLogic = new OptionsLogicLevel(levelUI.OptionsDialog, this);
            optionsLogic.InitDialog();
            optionsLogic.OnClose += UI_OnOptionsMenuCloseCallback;
        }
        private void ShowPausedDialog()
        {
            var levelUI = GetLevelUI();
            var spriteReference = pauseImages.Random(uiRandom);
            levelUI.SetPauseDialogActive(true);
            levelUI.SetPauseDialogImage(Main.LanguageManager.GetSprite(spriteReference));
        }
        private void ShowGameOverDialog()
        {
            string message;
            if (killerID != null)
            {
                var entityDef = Game.GetEntityDefinition(killerID);
                message = entityDef.GetDeathMessage();
            }
            else
            {
                message = deathMessage;
            }
            var levelUI = GetLevelUI();
            levelUI.SetGameOverDialogActive(true);
            levelUI.SetGameOverDialogMessage(Main.LanguageManager._p(StringTable.CONTEXT_DEATH_MESSAGE, message));
        }
        private void ShowLevelErrorLoadingDialog(string desc)
        {
            var levelUI = GetLevelUI();
            levelUI.SetLevelErrorLoadingDialogVisible(true);
            levelUI.SetLevelErrorLoadingDialogDesc(desc);
        }
        private void ShowLevelErrorLoadingDialog()
        {
            ShowLevelErrorLoadingDialog(Main.LanguageManager._(ERROR_LOAD_LEVEL_IDENTIFIER_NOT_MATCH));
        }
        private void ShowLevelLoadedDialog()
        {
            var ui = GetLevelUI();
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
                var modelID = entityID.ToModelID(ModelID.TYPE_ENTITY);
                sprite = Main.ResourceManager.GetModelIcon(modelID);
            }
            return sprite;
        }
        #endregion
        private void ClickPickaxe()
        {
            if (level.IsPickaxeDisabled())
                return;
            if (level.IsHoldingItem())
            {
                if (level.CancelHeldItem())
                {
                    level.PlaySound(SoundID.tap);
                }
                return;
            }
            level.PlaySound(SoundID.pickaxe);
            level.SetHeldItem(HeldTypes.pickaxe, 0, 0);
        }
        private void ClickStarshard()
        {
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
                //return;
            }
            level.SetHeldItem(HeldTypes.starshard, 0, 0);
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
                bannerProgresses[i] = Mathf.Clamp01(bannerProgresses[i] + level.CurrentWave >= i * level.GetWavesPerFlag() ? deltaTime : -deltaTime);
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
            var ui = GetLevelUI();
            ui.SetReadySetBuildVisible(true);
        }
        private void UpdateLevelUI(float simulationSpeed)
        {
            var ui = GetLevelUI();
            ui.SetEnergy(Mathf.FloorToInt(Mathf.Max(0, level.Energy - level.GetDelayedEnergy())).ToString());
            ui.SetPickaxeVisible(!level.IsHoldingPickaxe());
            ui.SetLevelTextAnimationSpeed(simulationSpeed);
            UpdateLevelProgress();
            UpdateBlueprintsState();
            UpdateStarshards();
        }
        private void UpdateLevelProgress()
        {
            var ui = GetLevelUI();
            ui.SetProgressVisible(level.LevelProgressVisible);
            ui.SetProgress(levelProgress);
            ui.SetBannerProgresses(bannerProgresses);
        }
        private void UpdateLevelName()
        {
            string name = level.GetLevelName();
            if (string.IsNullOrEmpty(name))
            {
                name = StringTable.LEVEL_NAME_UNKNOWN;
            }
            var levelUI = GetLevelUI();
            var levelName = Main.LanguageManager._p(StringTable.CONTEXT_LEVEL_NAME, name);
            levelUI.SetLevelName(levelName);
        }
        private void UpdateStarshards()
        {
            var levelUI = GetLevelUI();
            levelUI.SetStarshardCount(level.GetStarshardCount(), 3);
        }
        private void UpdateDifficultyName()
        {
            var difficultyName = Main.ResourceManager.GetDifficultyName(level.Difficulty);
            var levelUI = GetLevelUI();
            levelUI.SetDifficulty(difficultyName);
        }
        private void SetUIVisibleState(VisibleState state)
        {
            var levelUI = GetLevelUI();
            var toolsVisible = state == VisibleState.InLevel || state == VisibleState.ChoosingBlueprints;
            levelUI.SetEnergyVisible(toolsVisible);
            levelUI.SetBlueprintsVisible(toolsVisible);
            levelUI.SetPickaxeSlotVisible(toolsVisible);
            levelUI.SetTopRightVisible(toolsVisible);
            levelUI.SetTriggerSlotVisible(toolsVisible && Main.SaveManager.IsTriggerUnlocked());

            var inlevelVisible = state == VisibleState.InLevel;
            levelUI.SetStarshardVisible(inlevelVisible && Main.SaveManager.IsStarshardUnlocked());
            levelUI.SetSpeedUpVisible(inlevelVisible);
            levelUI.SetLevelNameVisible(inlevelVisible);
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
        private LevelUI standaloneUI;
        [SerializeField]
        private LevelUI mobileUI;
        [SerializeField]
        private List<SpriteReference> pauseImages = new List<SpriteReference>();
        #endregion

        #region 内嵌类
        public enum VisibleState
        {
            Nothing,
            ChoosingBlueprints,
            InLevel,
        }
        #endregion
    }
}
