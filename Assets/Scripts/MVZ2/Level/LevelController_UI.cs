using System;
using System.Collections.Generic;
using System.Linq;
using MukioI18n;
using MVZ2.Almanacs;
using MVZ2.GameContent.HeldItems;
using MVZ2.Level.UI;
using MVZ2.Options;
using MVZ2.UI;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.HeldItems;
using MVZ2.Vanilla.Level;
using MVZ2.Vanilla.Saves;
using MVZ2Logic.Callbacks;
using MVZ2Logic.HeldItems;
using MVZ2Logic.Level;
using PVZEngine;
using PVZEngine.Level;
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

        public void SetHeldItemUI(NamespaceID heldType, long id, int priority, bool noCancel)
        {
            Sprite icon = null;
            if (heldType == BuiltinHeldTypes.blueprint)
            {
                icon = GetHeldItemIcon(id);
            }
            else if (heldType == VanillaHeldTypes.pickaxe)
            {
                icon = Localization.GetSprite(pickaxeSprite);
            }
            else if (heldType == VanillaHeldTypes.starshard)
            {
                icon = Localization.GetSprite(GetStarshardIcon(level.AreaDefinition.GetID()));
            }
            else if (heldType == VanillaHeldTypes.trigger)
            {
                icon = Localization.GetSprite(triggerSprite);
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
            var definition = Game.GetHeldItemDefinition(heldType);
            LayerMask layerMask = Layers.GetMask(layers.ToArray());
            var uiPreset = GetUIPreset();
            uiPreset.SetRaycasterMask(layerMask);
            levelRaycaster.eventMask = layerMask;
            levelRaycaster.SetHeldItem(definition, id);
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
            var title = Localization._(Vanilla.VanillaStrings.RESTART);
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
        public bool IsChoosingBlueprints()
        {
            return isChoosingBlueprints;
        }
        public void UpdateDifficulty()
        {
            level.SetDifficulty(Options.GetDifficulty());
            UpdateDifficultyName();
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

            uiPreset.OnBlueprintChooseArtifactClick += UI_OnBlueprintChooseArtifactClickCallback;
            uiPreset.OnBlueprintChooseBlueprintPointerEnter += UI_OnBlueprintChooseBlueprintPointerEnterCallback;
            uiPreset.OnBlueprintChooseBlueprintPointerExit += UI_OnBlueprintChooseBlueprintPointerExitCallback;
            uiPreset.OnBlueprintChooseBlueprintPointerDown += UI_OnBlueprintChooseBlueprintPointerDownCallback;
            uiPreset.OnBlueprintChooseCommandBlockClick += UI_OnBlueprintChooseCommandBlockClickCallback;
            uiPreset.OnBlueprintChooseStartClick += UI_OnBlueprintChooseStartClickCallback;
            uiPreset.OnBlueprintChooseViewLawnClick += UI_OnBlueprintChooseViewLawnClickCallback;
            uiPreset.OnBlueprintChooseViewAlmanacClick += UI_OnBlueprintChooseViewAlmanacClickCallback;
            uiPreset.OnBlueprintChooseViewStoreClick += UI_OnBlueprintChooseViewStoreClickCallback;

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
            if (!IsGameStarted())
                return;

            ClickOnReceiver(receiver);
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
                name = Localization._(Vanilla.VanillaStrings.TOOLTIP_DIG_CONTRAPTION),
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
                Pause();
                level.PlaySound(VanillaSoundID.pause);
            }
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
            if (!IsGameStarted())
                return;
            ClickStarshard();
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

        #region 蓝图选择
        private async void UI_OnBlueprintChooseStartClickCallback()
        {
            if (chosenBlueprints.Count < level.GetSeedSlotCount())
            {
                var title = Localization._(Vanilla.VanillaStrings.WARNING);
                var desc = Localization._(WARNING_SELECTED_BLUEPRINTS_NOT_FULL);
                var result = await Scene.ShowDialogSelectAsync(title, desc);
                if (!result)
                    return;
            }
            isChoosingBlueprints = false;
            StartCoroutine(BlueprintChosenTransition());
        }
        private void UI_OnBlueprintChooseViewLawnClickCallback()
        {

        }
        private void UI_OnBlueprintChooseCommandBlockClickCallback()
        {

        }
        private void UI_OnBlueprintChooseViewAlmanacClickCallback()
        {
            OpenAlmanac(AlmanacUI.AlmanacPage.Index);
        }
        private void UI_OnBlueprintChooseViewStoreClickCallback()
        {

        }
        private void UI_OnBlueprintChooseBlueprintPointerEnterCallback(int index, PointerEventData eventData)
        {
            var uiPreset = GetUIPreset();
            var blueprintID = choosingBlueprints[index];
            GetBlueprintTooltip(blueprintID, out var name, out var tooltip);
            string error = null;
            if (!IsChoosingBlueprintError(blueprintID, out var errorMessage) && !string.IsNullOrEmpty(errorMessage))
            {
                error = Localization._(errorMessage);
            }
            var viewData = new TooltipViewData()
            {
                name = name,
                error = error,
                description = tooltip
            };
            uiPreset.ShowTooltipOnChoosingBlueprint(index, viewData);
        }
        private void UI_OnBlueprintChooseBlueprintPointerExitCallback(int index, PointerEventData eventData)
        {
            var uiPreset = GetUIPreset();
            uiPreset.HideTooltip();
        }
        private void UI_OnBlueprintChooseBlueprintPointerDownCallback(int index, PointerEventData eventData)
        {
            var seedSlots = level.GetSeedSlotCount();
            var seedPackIndex = chosenBlueprints.Count;
            if (seedPackIndex >= seedSlots)
                return;
            if (chosenBlueprints.Contains(index))
                return;
            chosenBlueprints.Add(index);

            // 更新UI。
            var uiPreset = GetUIPreset();
            UpdateBlueprintChooseItem(index);

            // 更新蓝图贴图。
            var seedID = choosingBlueprints[index];
            var seedDef = Game.GetSeedDefinition(seedID);
            var blueprint = uiPreset.CreateBlueprint();
            var blueprintViewData = Resources.GetBlueprintViewData(seedDef);
            blueprint.UpdateView(blueprintViewData);
            blueprint.SetDisabled(false);
            blueprint.SetRecharge(0);
            blueprint.SetSelected(false);
            blueprint.SetTwinkling(false);

            // 将蓝图移动到目标位置。
            var choosingBlueprintItem = uiPreset.GetBlueprintChooseItem(index);
            blueprint.transform.position = choosingBlueprintItem.transform.position;
            var startPos = blueprint.transform.position;
            var targetPos = uiPreset.GetBlueprintPosition(seedPackIndex);
            var movingBlueprint = uiPreset.CreateMovingBlueprint();
            movingBlueprint.transform.position = startPos;
            movingBlueprint.SetBlueprint(blueprint);
            movingBlueprint.SetMotion(startPos, targetPos);
            movingBlueprint.OnMotionFinished += () =>
            {
                uiPreset.InsertBlueprint(seedPackIndex, blueprint);
                uiPreset.RemoveMovingBlueprint(movingBlueprint);
            };

            // 播放音效。
            level.PlaySound(VanillaSoundID.tap);
        }
        private void UI_OnBlueprintChooseArtifactClickCallback(int index)
        {

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
            var spriteReference = pauseImages.Random(rng);
            ui.SetPauseDialogActive(true);
            ui.SetPauseDialogImage(Localization.GetSprite(spriteReference));
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
            ui.SetGameOverDialogMessage(Localization._p(Vanilla.VanillaStrings.CONTEXT_DEATH_MESSAGE, message));
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
        #endregion

        #region 选择蓝图
        private void ShowBlueprintChoosePanel(IEnumerable<NamespaceID> blueprints)
        {
            var panelViewData = new BlueprintChoosePanelViewData()
            {
                hasArtifacts = false,
                canViewLawn = level.CurrentFlag > 0,
                hasCommandBlock = false,
            };
            var orderedBlueprints = new List<NamespaceID>();
            Main.AlmanacManager.GetOrderedBlueprints(blueprints, orderedBlueprints);
            var blueprintViewDatas = orderedBlueprints.Select(id => Main.AlmanacManager.GetChoosingBlueprintViewData(id)).ToArray();
            isChoosingBlueprints = true;
            choosingBlueprints = orderedBlueprints.ToArray();

            var uiPreset = GetUIPreset();
            uiPreset.SetBlueprintChooseViewAlmanacButtonActive(Saves.IsAlmanacUnlocked());
            uiPreset.SetBlueprintChooseViewStoreButtonActive(Saves.IsStoreUnlocked());
            uiPreset.SetSideUIVisible(true);
            uiPreset.SetBlueprintsChooseVisible(true);
            uiPreset.ResetBlueprintChooseArtifactCount(3);
            uiPreset.UpdateBlueprintChooseElements(panelViewData);
            uiPreset.UpdateBlueprintChooseItems(blueprintViewDatas);
            uiPreset.SetUIVisibleState(VisibleState.ChoosingBlueprints);

            UpdateChosenBlueprints();
        }
        private void UpdateChosenBlueprints()
        {
            var uiPreset = GetUIPreset();
            var slotCount = level.GetSeedSlotCount();
            for (int i = 0; i < slotCount; i++)
            {
                BlueprintViewData viewData = BlueprintViewData.Empty;
                if (i < chosenBlueprints.Count)
                {
                    var blueprintIndex = chosenBlueprints[i];
                    var blueprint = choosingBlueprints[blueprintIndex];
                    var seedDef = Game.GetSeedDefinition(blueprint);
                    if (seedDef != null)
                    {
                        viewData = Resources.GetBlueprintViewData(seedDef);
                    }
                }
                var blueprintUI = uiPreset.GetBlueprintAt(i);
                if (blueprintUI)
                {
                    blueprintUI.UpdateView(viewData);
                    blueprintUI.SetDisabled(false);
                    blueprintUI.SetRecharge(0);
                    blueprintUI.SetSelected(false);
                    blueprintUI.SetTwinkling(false);
                }
            }
        }
        private void UpdateBlueprintChooseItem(int index)
        {
            var uiPreset = GetUIPreset();
            var blueprintChooseItem = uiPreset.GetBlueprintChooseItem(index);
            bool selected = chosenBlueprints.Contains(index);
            blueprintChooseItem.SetDisabled(selected);
            blueprintChooseItem.SetRecharge(selected ? 1 : 0);
        }
        private bool IsChoosingBlueprintError(NamespaceID id, out string errorMessage)
        {
            errorMessage = null;
            return true;
        }
        #endregion

        #region 图鉴
        private void OpenAlmanac(AlmanacUI.AlmanacPage page)
        {
            isOpeningAlmanac = true;
            levelCamera.gameObject.SetActive(false);
            Main.Scene.DisplayAlmanac(() =>
            {
                isOpeningAlmanac = false;
                levelCamera.gameObject.SetActive(true);
            });
        }
        private void OpenEnemyAlmanac(NamespaceID enemyID)
        {
            OpenAlmanac(AlmanacUI.AlmanacPage.Enemies);
            Main.Scene.DisplayEnemyAlmanac(enemyID);
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
            level.SetHeldItem(VanillaHeldTypes.trigger, 0, 0);
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
        private void PlayReadySetBuild()
        {
            var ui = GetUIPreset();
            ui.ShowReadySetBuild();
        }
        private void UpdateLevelUI()
        {
            var ui = GetUIPreset();
            ui.SetMoney((level.GetMoney() - level.GetDelayedMoney()).ToString("N0"));
            ui.SetPickaxeVisible(!level.IsHoldingPickaxe());
            UpdateEnergy();
            UpdateLevelProgress();
            UpdateBlueprintsState();
            UpdateStarshards();
        }
        private void UpdateEnergy()
        {
            var ui = GetUIPreset();
            ui.SetEnergy(Mathf.FloorToInt(Mathf.Max(0, level.Energy - level.GetDelayedEnergy())).ToString());
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
                name = Vanilla.VanillaStrings.LEVEL_NAME_UNKNOWN;
            }
            var levelUI = GetUIPreset();
            var levelName = Localization._p(Vanilla.VanillaStrings.CONTEXT_LEVEL_NAME, name);
            if (dayNumber > 0)
            {
                levelName = Localization._p(Vanilla.VanillaStrings.CONTEXT_LEVEL_NAME, Vanilla.VanillaStrings.LEVEL_NAME_DAY_TEMPLATE, levelName, dayNumber);
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
            var difficultyName = Resources.GetDifficultyName(level.Difficulty);
            var levelUI = GetUIPreset();
            levelUI.SetDifficulty(difficultyName);
        }
        private void SetUIVisibleState(VisibleState state)
        {
            var levelUI = GetUIPreset();
            levelUI.SetUIVisibleState(state);
        }
        private void SetUnlockedUIActive()
        {
            var levelUI = GetUIPreset();
            StarshardActive = Saves.IsStarshardUnlocked();
            TriggerActive = Saves.IsTriggerUnlocked();
        }
        private NamespaceID GetStarshardIcon(NamespaceID areaID)
        {
            var spriteID = new NamespaceID(areaID.spacename, $"starshards/{areaID.path}");
            if (!Resources.GetSprite(spriteID))
            {
                spriteID = new NamespaceID(areaID.spacename, $"starshards/default");
            }
            return spriteID;
        }

        #endregion

        #region 属性字段

        [TranslateMsg("对话框内容")]
        public const string DIALOG_DESC_RESTART = "确认要重新开始关卡吗？\n本关的进度都将丢失。";
        [TranslateMsg("对话框内容，{0}为错误信息")]
        public const string ERROR_LOAD_LEVEL_EXCEPTION = "加载关卡失败，出现错误：{0}";
        [TranslateMsg("对话框内容")]
        public const string ERROR_LOAD_LEVEL_IDENTIFIER_NOT_MATCH = "加载关卡失败，存档状态和当前游戏状态不匹配。";
        [TranslateMsg("对话框内容")]
        public const string WARNING_SELECTED_BLUEPRINTS_NOT_FULL = "你没有携带满蓝图，确认要继续吗？";

        #region 保存属性
        private float levelProgress;
        private float[] bannerProgresses;
        #endregion

        public RandomGenerator RNG => rng;
        private RandomGenerator rng = new RandomGenerator(Guid.NewGuid().GetHashCode());
        private OptionsLogicLevel optionsLogic;
        private bool isChoosingBlueprints;
        private List<int> chosenBlueprints = new List<int>();
        private NamespaceID[] choosingBlueprints;

        [Header("UI")]
        [SerializeField]
        private LevelRaycaster levelRaycaster;
        [SerializeField]
        private LevelUIPreset standaloneUI;
        [SerializeField]
        private LevelUIPreset mobileUI;
        [SerializeField]
        private List<Sprite> pauseImages = new List<Sprite>();
        [SerializeField]
        private Sprite pickaxeSprite;
        [SerializeField]
        private Sprite triggerSprite;
        #endregion
    }
}
