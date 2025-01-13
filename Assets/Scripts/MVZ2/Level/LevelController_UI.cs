using System;
using System.Collections.Generic;
using System.Linq;
using MukioI18n;
using MVZ2.Cursors;
using MVZ2.GameContent.HeldItems;
using MVZ2.HeldItems;
using MVZ2.Level.UI;
using MVZ2.Models;
using MVZ2.Options;
using MVZ2.UI;
using MVZ2.Vanilla;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Contraptions;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.HeldItems;
using MVZ2.Vanilla.Level;
using MVZ2.Vanilla.Saves;
using MVZ2.Vanilla.SeedPacks;
using MVZ2Logic;
using MVZ2Logic.Artifacts;
using MVZ2Logic.Games;
using MVZ2Logic.HeldItems;
using MVZ2Logic.Level;
using PVZEngine;
using PVZEngine.Entities;
using PVZEngine.Level;
using PVZEngine.Models;
using PVZEngine.SeedPacks;
using Tools;
using UnityEngine;
using UnityEngine.EventSystems;
using static MVZ2.Level.UI.LevelUIPreset;

namespace MVZ2.Level
{
    public partial class LevelController : MonoBehaviour, IDisposable
    {
        #region 公有方法

        public LevelUIPreset GetUIPreset()
        {
            return ui.GetUIPreset();
        }

        #region 手持物品
        public void SetHeldItemModel(NamespaceID modelID)
        {
            Model model = null;
            var modelMeta = Main.ResourceManager.GetModelMeta(modelID);
            if (modelMeta != null)
            {
                model = Main.ResourceManager.GetModel(modelMeta.Path);
            }
            ui.SetHeldItemModel(model, GetCamera());
        }
        public Model GetHeldItemModel()
        {
            return ui.GetHeldItemModel();
        }
        public void SetHeldItemUI(IHeldItemData data)
        {
            NamespaceID heldType = data.Type;
            bool instantTrigger = data.InstantTrigger;

            var definition = Game.GetHeldItemDefinition(heldType);

            // 设置图标。
            var modelID = definition?.GetModelID(level, data);
            SetHeldItemModel(modelID);

            // 显示触发器图标。
            bool triggerVisible = false;
            var blueprint = definition.GetSeedPack(level, data);
            if (blueprint != null && blueprint.IsTriggerActive() && blueprint.CanInstantTrigger())
            {
                triggerVisible = true;
            }
            ui.SetHeldItemTrigger(triggerVisible, instantTrigger);

            // 设置射线检测图层。
            List<int> layers = new List<int>();
            layers.Add(Layers.RAYCAST_RECEIVER);
            layers.Add(Layers.GRID);
            layers.Add(Layers.DEFAULT);
            layers.Add(Layers.PICKUP);
            LayerMask layerMask = Layers.GetMask(layers.ToArray());

            var uiPreset = GetUIPreset();
            uiPreset.SetRaycasterMask(layerMask);
            levelRaycaster.eventMask = layerMask;

            // 设置射线检测半径。
            var radius = (definition?.GetRadius(level, data) ?? 0) * LawnToTransScale;
            levelRaycaster.SetHeldItem(definition, data, radius);

            // 设置光标。
            bool isHeldItemNone = heldType == BuiltinHeldTypes.none || !NamespaceID.IsValid(modelID);
            if (isHeldItemNone)
            {
                if (heldItemCursorSource != null)
                {
                    Main.CursorManager.RemoveCursorSource(heldItemCursorSource);
                    heldItemCursorSource = null;
                }
            }
            else
            {
                if (heldItemCursorSource == null)
                {
                    heldItemCursorSource = new HeldItemCursorSource(this);
                    Main.CursorManager.AddCursorSource(heldItemCursorSource);
                }
            }
        }
        public IModelInterface GetHeldItemModelInterface()
        {
            return heldItemModelInterface;
        }
        #endregion

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
        #endregion

        #region 对话框
        public void ShowRestartConfirmDialog()
        {
            var title = Localization._(VanillaStrings.RESTART);
            var desc = Localization._(DIALOG_DESC_RESTART);
            Scene.ShowDialogSelect(title, desc, async (confirm) =>
            {
                if (confirm)
                {
                    await RestartLevel();
                }
            });
        }
        public void ShowLevelErrorLoadingDialog(Exception e)
        {
            ShowLevelErrorLoadingDialog(Localization._(ERROR_LOAD_LEVEL_EXCEPTION, e.Message));
        }
        #endregion

        #region 选择蓝图
        public bool CanChooseBlueprints()
        {
            return BlueprintChoosePart.IsInteractable();
        }
        #endregion

        #region 难度
        public void UpdateDifficulty()
        {
            if (level.CurrentFlag <= 0)
            {
                level.SetDifficulty(Options.GetDifficulty());
            }
            UpdateDifficultyName();
        }
        #endregion

        #region 进度条
        public void SetProgressToBoss(NamespaceID barStyleID)
        {
            var ui = GetUIPreset();
            progressBarMode = true;
            bossProgressBarStyle = barStyleID;
            ui.SetProgressBarMode(progressBarMode);
            var meta = Main.ResourceManager.GetProgressBarMeta(barStyleID);
            if (meta == null)
                return;
            var background = Main.GetFinalSprite(meta.BackgroundSprite);
            var foreground = Main.GetFinalSprite(meta.ForegroundSprite);
            var bar = Main.GetFinalSprite(meta.BarSprite);
            var icon = Main.GetFinalSprite(meta.IconSprite);
            var viewData = new ProgressBarTemplateViewData()
            {
                backgroundSprite = background,
                foregroundSprite = foreground,
                barSprite = bar,
                fromLeft = meta.FromLeft,
                iconSprite = icon,
                padding = meta.Padding,
                size = meta.Size,
            };
            ui.SetBossProgressTemplate(viewData);
        }
        public void SetProgressToStage()
        {
            var ui = GetUIPreset();
            progressBarMode = false;
            ui.SetProgressBarMode(progressBarMode);
        }
        #endregion

        public void ShowTooltipOnComponent(ITooltipTarget target, TooltipViewData viewData)
        {
            var uiPreset = GetUIPreset();
            uiPreset.ShowTooltipOnComponent(target, viewData);
        }
        public void HideTooltip()
        {
            var uiPreset = GetUIPreset();
            uiPreset.HideTooltip();
        }
        public bool IsTriggerSwapped()
        {
            return Main.SaveManager.IsUnlocked(VanillaUnlockID.trigger) && Main.OptionsManager.IsTriggerSwapped();
        }
        public Vector3 ScreenToLawnPositionByZ(Vector2 screenPosition, float z)
        {
            var worldPosition = levelCamera.Camera.ScreenToWorldPoint(screenPosition);
            worldPosition.z = 0;

            var lawnPosition = TransToLawn(worldPosition);
            lawnPosition.z = z;
            lawnPosition.y -= z;

            return lawnPosition;
        }
        public Vector3 ScreenToLawnPositionByY(Vector2 screenPosition, float y)
        {
            var worldPosition = levelCamera.Camera.ScreenToWorldPoint(screenPosition);
            worldPosition.z = 0;

            var lawnPosition = TransToLawn(worldPosition);
            lawnPosition.z = lawnPosition.y - y;
            lawnPosition.y = y;

            return lawnPosition;
        }
        public Vector3 ScreenToLawnPositionByRelativeY(Vector2 screenPosition, float relativeY)
        {
            var worldPosition = levelCamera.Camera.ScreenToWorldPoint(screenPosition);
            worldPosition.z = 0;

            var lawnPosition = TransToLawn(worldPosition);
            var targetY = lawnPosition.y;
            var x = lawnPosition.x;
            var currentZ = targetY;
            for (int i = 1; i < 8; i++)
            {
                var yOffset = level.GetGroundY(x, currentZ) + relativeY;
                var result = yOffset + currentZ;
                currentZ = currentZ + (currentZ - result) * 0.5f;
            }
            lawnPosition.z = currentZ;
            lawnPosition.y = targetY - currentZ;

            return lawnPosition;
        }

        #endregion

        #region 私有方法

        private void Awake_UI()
        {
            heldItemModelInterface = new HeldItemModelInterface(this);
            ui.OnPauseDialogResumeClicked += UI_OnPauseDialogResumeClickedCallback;
            ui.OnLevelLoadedDialogButtonClicked += UI_OnLevelLoadedDialogOptionClickedCallback;
            ui.OnLevelErrorLoadingDialogButtonClicked += UI_OnLevelErrorLoadingDialogOptionClickedCallback;

            ui.OnGameOverRetryButtonClicked += UI_OnGameOverRetryButtonClickedCallback;
            ui.OnGameOverBackButtonClicked += UI_OnGameOverBackButtonClickedCallback;

            ui.SetHeldItemModel(null, GetCamera());
            ui.SetPauseDialogActive(false);
            ui.SetOptionsDialogActive(false);
            ui.SetGameOverDialogActive(false);
            ui.SetLevelLoadedDialogVisible(false);
            ui.SetLevelErrorLoadingDialogVisible(false);
            ui.BlueprintChoose.SetViewLawnReturnBlockerActive(false);

            var uiPreset = GetUIPreset();
            uiPreset.OnPickaxePointerEnter += UI_OnPickaxePointerEnterCallback;
            uiPreset.OnPickaxePointerExit += UI_OnPickaxePointerExitCallback;
            uiPreset.OnPickaxePointerDown += UI_OnPickaxePointerDownCallback;

            uiPreset.OnStarshardPointerDown += UI_OnStarshardPointerDownCallback;

            uiPreset.OnTriggerPointerEnter += UI_OnTriggerPointerEnterCallback;
            uiPreset.OnTriggerPointerExit += UI_OnTriggerPointerExitCallback;
            uiPreset.OnTriggerPointerDown += UI_OnTriggerPointerDownCallback;

            uiPreset.OnRaycastReceiverPointerDown += UI_OnRaycastReceiverPointerDownCallback;
            uiPreset.OnMenuButtonClick += UI_OnMenuButtonClickCallback;
            uiPreset.OnSpeedUpButtonClick += UI_OnSpeedUpButtonClickCallback;

            uiPreset.OnArtifactPointerEnter += UI_OnArtifactPointerEnterCallback;
            uiPreset.OnArtifactPointerExit += UI_OnArtifactPointerExitCallback;

            uiPreset.HideMoney();
            uiPreset.SetProgressBarVisible(false);
            uiPreset.HideTooltip();
            SetUIVisibleState(VisibleState.Nothing);
            ui.Blueprints.SetConveyorMode(false);
        }

        #region 事件回调

        #region UI方
        private void UI_OnRaycastReceiverPointerDownCallback(LevelUIPreset.Receiver receiver, PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;
            if (!IsGameStarted())
                return;

            ClickOnReceiver(receiver, PointerPhase.Press);
        }
        private void UI_OnPickaxePointerEnterCallback(PointerEventData eventData)
        {
            if (!IsGameStarted())
                return;
            var levelUI = GetUIPreset();
            string error = null;
            if (level.IsPickaxeDisabled())
            {
                var message = level.GetPickaxeDisableMessage();
                if (!string.IsNullOrEmpty(message))
                {
                    error = Localization._(message);
                }
            }
            var viewData = new TooltipViewData()
            {
                name = Localization._(VanillaStrings.TOOLTIP_DIG_CONTRAPTION),
                error = error,
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
            if (!IsGameStarted())
                return;
            ClickPickaxe();
        }
        private void UI_OnMenuButtonClickCallback()
        {
            if (IsGameRunning())
            {
                PauseGame();
                level.PlaySound(VanillaSoundID.pause);
            }
            ShowOptionsDialog();
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
                ResumeGame(100);
            }
        }
        private void UI_OnSpeedUpButtonClickCallback()
        {
            SwitchSpeedUp();
        }
        private void UI_OnStarshardPointerDownCallback(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;
            if (!IsGameStarted())
                return;
            ClickStarshard();
        }
        private void UI_OnTriggerPointerEnterCallback(PointerEventData eventData)
        {
            if (!IsGameStarted())
                return;
            var levelUI = GetUIPreset();
            string error = null;
            if (level.IsTriggerDisabled())
            {
                var message = level.GetTriggerDisableMessage();
                if (!string.IsNullOrEmpty(message))
                {
                    error = Localization._(message);
                }
            }
            var viewData = new TooltipViewData()
            {
                name = Localization._(VanillaStrings.TOOLTIP_TRIGGER_CONTRAPTION),
                error = error,
                description = null
            };
            levelUI.ShowTooltipOnTrigger(viewData);
        }
        private void UI_OnTriggerPointerExitCallback(PointerEventData eventData)
        {
            var levelUI = GetUIPreset();
            levelUI.HideTooltip();
        }
        private void UI_OnTriggerPointerDownCallback(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;
            if (!IsGameStarted())
                return;
            ClickTrigger();
        }
        private void UI_OnPauseDialogResumeClickedCallback()
        {
            ResumeGame(100);
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
                    ResumeGame(100);
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

        #region 制品
        private void UI_OnArtifactPointerEnterCallback(int index)
        {
            var uiPreset = GetUIPreset();
            var artifact = level.GetArtifactAt(index);
            if (artifact?.Definition == null)
                return;
            var artifactID = artifact.Definition.GetID();
            var name = Main.ResourceManager.GetArtifactName(artifactID);
            var tooltip = Main.ResourceManager.GetArtifactTooltip(artifactID);
            TooltipViewData viewData = new TooltipViewData()
            {
                name = name,
                error = string.Empty,
                description = tooltip
            };
            uiPreset.ShowTooltipOnComponent(uiPreset.GetArtifactAt(index), viewData);
        }
        private void UI_OnArtifactPointerExitCallback(int index)
        {
            var uiPreset = GetUIPreset();
            uiPreset.HideTooltip();
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
            var sprite = pauseImages.Random(rng);
            ui.SetPauseDialogActive(true);
            ui.SetPauseDialogImage(Main.GetFinalSprite(sprite));
        }
        private void ShowGameOverDialog()
        {
            string message;
            if (killerID != null)
            {
                message = Resources.GetEntityDeathMessage(killerID);
            }
            else
            {
                message = deathMessage;
            }
            ui.SetGameOverDialogActive(true);
            ui.SetGameOverDialogMessage(Localization._p(VanillaStrings.CONTEXT_DEATH_MESSAGE, message));
        }
        private void ShowLevelErrorLoadingDialog(string desc)
        {
            ui.SetLevelErrorLoadingDialogVisible(true);
            ui.SetLevelErrorLoadingDialogDesc(desc);
        }
        private void ShowLevelErrorLoadingDialog()
        {
            ShowLevelErrorLoadingDialog(Localization._(ERROR_LOAD_LEVEL_IDENTIFIER_NOT_MATCH));
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
            return Resources.GetBlueprintIconStandalone(seedDef);
        }
        private void UpdateHeldItemPosition()
        {
            bool isPressing = Input.touchCount > 0 || Input.GetMouseButton(0);
            Vector2 heldItemPosition;
            if (Main.IsMobile() && !isPressing && !level.KeepHeldItemInScreen())
            {
                heldItemPosition = new Vector2(-1000, -1000);
            }
            else
            {
                heldItemPosition = levelCamera.Camera.ScreenToWorldPoint(Input.mousePosition);
            }
            ui.SetHeldItemPosition(heldItemPosition);
        }
        private void UpdateHeldItemCursor()
        {
            var enabled = IsGameRunning() && (level != null && !level.IsCleared);
            if (heldItemCursorSource != null && enabled != heldItemCursorSource.Enabled)
            {
                heldItemCursorSource.SetEnabled(enabled);
            }
        }
        private void UpdateHeldSlotUI()
        {
            bool pickaxeDisabled = level.IsPickaxeDisabled();
            bool starshardDisabled = level.IsStarshardDisabled();
            var uiPreset = GetUIPreset();
            uiPreset.SetStarshardSelected(level.IsHoldingStarshard());
            uiPreset.SetStarshardDisabled(starshardDisabled && level.ShouldShowStarshardDisableIcon());
            uiPreset.SetPickaxeSelected(level.IsHoldingPickaxe());
            uiPreset.SetPickaxeDisabled(pickaxeDisabled && level.ShouldShowPickaxeDisableIcon());
            uiPreset.SetTriggerSelected(level.IsHoldingTrigger());
        }
        private void ValidateHeldItem()
        {
            bool pickaxeDisabled = level.IsPickaxeDisabled();
            bool starshardDisabled = level.IsStarshardDisabled();
            if (pickaxeDisabled && level.IsHoldingPickaxe())
            {
                level.ResetHeldItem();
            }
            if (starshardDisabled && level.IsHoldingStarshard())
            {
                level.ResetHeldItem();
            }
        }
        #endregion


        #region 图鉴
        public void OpenAlmanac()
        {
            isOpeningAlmanac = true;
            levelCamera.gameObject.SetActive(false);
            Main.Scene.DisplayAlmanac(() =>
            {
                isOpeningAlmanac = false;
                levelCamera.gameObject.SetActive(true);
                if (!Music.IsPlaying(VanillaMusicID.choosing))
                    Music.Play(VanillaMusicID.choosing);
            });
        }
        private void OpenEnemyAlmanac(NamespaceID enemyID)
        {
            OpenAlmanac();
            Main.Scene.DisplayEnemyAlmanac(enemyID);
        }
        #endregion

        #region 商店
        public void OpenStore()
        {
            isOpeningStore = true;
            levelCamera.gameObject.SetActive(false);
            Main.Scene.DisplayStore(() =>
            {
                isOpeningStore = false;
                levelCamera.gameObject.SetActive(true);
                level.UpdatePersistentLevelUnlocks();
                BlueprintChoosePart.UpdateChosenArtifacts();
                if (!Music.IsPlaying(VanillaMusicID.choosing))
                    Music.Play(VanillaMusicID.choosing);
            });
        }
        #endregion

        #region 可操作工具
        private void ClickPickaxe()
        {
            if (!PickaxeActive)
                return;
            if (level.IsHoldingItem())
            {
                if (level.CancelHeldItem())
                {
                    level.PlaySound(VanillaSoundID.tap);
                }
                return;
            }
            if (level.IsPickaxeDisabled())
                return;
            level.PlaySound(VanillaSoundID.pickaxe);
            level.SetHeldItem(VanillaHeldTypes.pickaxe, 0, 0);
        }
        private void ClickStarshard()
        {
            if (!StarshardActive)
                return;
            if (level.IsHoldingItem())
            {
                if (level.CancelHeldItem())
                {
                    level.PlaySound(VanillaSoundID.tap);
                }
                return;
            }
            if (level.GetStarshardCount() <= 0)
            {
                level.PlaySound(VanillaSoundID.buzzer);
                return;
            }
            if (level.IsStarshardDisabled())
                return;
            level.SetHeldItem(VanillaHeldTypes.starshard, 0, 0);
        }
        private void ClickTrigger()
        {
            if (!TriggerActive)
                return;
            if (level.IsHoldingItem())
            {
                if (level.CancelHeldItem())
                {
                    level.PlaySound(VanillaSoundID.tap);
                }
                return;
            }
            if (level.IsTriggerDisabled())
                return;
            level.SetHeldItem(VanillaHeldTypes.trigger, 0, 0);
        }
        private void UpdateStarshards()
        {
            var levelUI = GetUIPreset();
            levelUI.SetStarshardCount(level.GetStarshardCount(), 3);
        }
        private void SetStarshardIcon()
        {
            var levelUI = GetUIPreset();
            var spriteRef = level.GetStarshardIcon();
            var sprite = Main.GetFinalSprite(spriteRef);
            levelUI.SetStarshardIcon(sprite);
        }
        private void SetUnlockedUIActive()
        {
            var levelUI = GetUIPreset();
            StarshardActive = Saves.IsStarshardUnlocked();
            TriggerActive = Saves.IsTriggerUnlocked();
        }
        #endregion

        #region 战场射线接收器
        private void ClickOnReceiver(RaycastReceiver receiver, PointerPhase phase)
        {
            var levelUI = GetUIPreset();
            var type = levelUI.GetReceiverType(receiver);
            ClickOnReceiver(type, phase);
        }
        private void ClickOnReceiver(LevelUIPreset.Receiver receiver, PointerPhase phase)
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
            var target = new HeldItemTargetLawn(level, area);
            level.UseHeldItem(target, phase);
        }
        #endregion

        #region 进度条
        private void AdvanceLevelProgress()
        {
            var deltaTime = Time.deltaTime;
            if (progressBarMode)
            {
                // BOSS血条
                var bosses = level.FindEntities(e => e.Type == EntityTypes.BOSS && e.IsHostileEnemy());
                if (bosses.Count() <= 0)
                {
                    bossProgress = 0;
                }
                else
                {
                    bossProgress = bosses.Sum(b => b.Health) / bosses.Sum(b => b.GetMaxHealth());
                }
            }
            else
            {
                // 关卡进度
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
                    float value = (level.CurrentWave >= (totalFlags - i) * level.GetWavesPerFlag()) ? deltaTime : -deltaTime;
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
        }
        private void UpdateLevelProgressUI()
        {
            var ui = GetUIPreset();
            ui.SetProgressBarVisible(level.LevelProgressVisible);
            ui.SetLevelProgress(levelProgress);
            ui.SetBannerProgresses(bannerProgresses);
            ui.SetBossProgress(bossProgress);
        }
        private void RefreshProgressBar()
        {
            if (progressBarMode)
            {
                SetProgressToBoss(bossProgressBarStyle);
            }
            else
            {
                SetProgressToStage();
            }
        }
        #endregion

        #region 关卡名
        private void UpdateLevelName()
        {
            var levelUI = GetUIPreset();
            levelUI.SetLevelName(LevelManager.GetStageName(level));
        }
        #endregion

        #region 提示红字
        private void PlayReadySetBuild()
        {
            var ui = GetUIPreset();
            ui.ShowReadySetBuild();
        }
        #endregion

        #region 金钱
        private void UpdateMoney()
        {
            var ui = GetUIPreset();
            ui.SetMoney((level.GetMoney() - level.GetDelayedMoney()).ToString("N0"));
        }
        #endregion

        #region 能量
        private void UpdateEnergy()
        {
            var ui = GetUIPreset();
            ui.SetEnergy(Mathf.FloorToInt(Mathf.Max(0, level.Energy - level.GetDelayedEnergy())).ToString());
        }
        #endregion

        #region 难度
        private void UpdateDifficultyName()
        {
            var difficultyName = Resources.GetDifficultyName(level.Difficulty);
            var levelUI = GetUIPreset();
            levelUI.SetDifficulty(difficultyName);
        }
        #endregion

        #region UI可见度
        private void SetUIVisibleState(VisibleState state)
        {
            var levelUI = GetUIPreset();
            levelUI.SetUIVisibleState(state);
        }
        #endregion


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
        #endregion

        #region 属性字段

        [TranslateMsg("对话框内容")]
        public const string DIALOG_DESC_RESTART = "确认要重新开始关卡吗？\n本关的进度都将丢失。";
        [TranslateMsg("对话框内容，{0}为错误信息")]
        public const string ERROR_LOAD_LEVEL_EXCEPTION = "加载关卡失败，出现错误：{0}";
        [TranslateMsg("对话框内容")]
        public const string ERROR_LOAD_LEVEL_IDENTIFIER_NOT_MATCH = "加载关卡失败，存档状态和当前游戏状态不匹配。";

        #region 保存属性
        private float levelProgress;
        private float[] bannerProgresses;
        private float bossProgress;
        private bool progressBarMode;
        private NamespaceID bossProgressBarStyle;
        #endregion

        public RandomGenerator RNG => rng;
        private RandomGenerator rng;
        private OptionsLogicLevel optionsLogic;
        private IModelInterface heldItemModelInterface;
        private CursorSource heldItemCursorSource;

        [Header("UI")]
        [SerializeField]
        private LevelRaycaster levelRaycaster;
        [SerializeField]
        private List<Sprite> pauseImages = new List<Sprite>();
        [SerializeField]
        private Sprite pickaxeSprite;
        [SerializeField]
        private Sprite triggerSprite;
        #endregion
    }
}
