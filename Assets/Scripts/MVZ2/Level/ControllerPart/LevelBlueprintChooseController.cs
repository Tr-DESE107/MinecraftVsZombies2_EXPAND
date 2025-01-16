using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using MukioI18n;
using MVZ2.GameContent.Effects;
using MVZ2.Games;
using MVZ2.Level.UI;
using MVZ2.UI;
using MVZ2.Vanilla;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Saves;
using MVZ2.Vanilla.SeedPacks;
using MVZ2Logic.Artifacts;
using MVZ2Logic.Callbacks;
using MVZ2Logic.Games;
using MVZ2Logic.Level;
using MVZ2Logic.SeedPacks;
using PVZEngine;
using PVZEngine.Base;
using PVZEngine.Definitions;
using PVZEngine.Level;
using PVZEngine.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MVZ2.Level
{
    public interface ILevelBlueprintChooseController : ILevelControllerPart
    {
        void ApplyChoose();
        void ShowBlueprintChoosePanel(IEnumerable<NamespaceID> blueprints);
        void DestroyChosenBlueprintUIAt(int index);
        string GetBlueprintTooltipError(NamespaceID blueprintID);
        string GetBlueprintTooltip(NamespaceID blueprintID);
        bool IsInteractable();
        void UnchooseBlueprint(int index);
        void Refresh(IEnumerable<NamespaceID> blueprints);
    }
    public class LevelBlueprintChooseController : LevelControllerPart, ILevelBlueprintChooseController
    {
        public override void Init(LevelController controller)
        {
            base.Init(controller);
            chooseUI.OnBlueprintItemPointerEnter += UI_OnBlueprintPointerEnterCallback;
            chooseUI.OnBlueprintItemPointerExit += UI_OnBlueprintPointerExitCallback;
            chooseUI.OnBlueprintItemPointerDown += UI_OnBlueprintPointerDownCallback;

            chooseUI.OnArtifactSlotClick += UI_OnArtifactSlotClickCallback;
            chooseUI.OnArtifactSlotPointerEnter += UI_OnArtifactSlotPointerEnterCallback;
            chooseUI.OnArtifactSlotPointerExit += UI_OnArtifactSlotPointerExitCallback;

            chooseUI.OnCommandBlockClick += UI_OnCommandBlockClickCallback;
            chooseUI.OnStartClick += UI_OnStartClickCallback;
            chooseUI.OnViewLawnClick += UI_OnViewLawnClickCallback;
            chooseUI.OnViewAlmanacClick += UI_OnViewAlmanacClickCallback;
            chooseUI.OnViewStoreClick += UI_OnViewStoreClickCallback;

            chooseUI.OnViewLawnReturnClick += UI_OnViewLawnReturnClickCallback;

            chooseUI.OnArtifactChoosingItemClicked += UI_OnArtifactChooseItemClickCallback;
            chooseUI.OnArtifactChoosingItemEnter += UI_OnArtifactChooseItemPointerEnterCallback;
            chooseUI.OnArtifactChoosingItemExit += UI_OnArtifactChooseItemPointerExitCallback;
            chooseUI.OnArtifactChoosingBackClicked += UI_OnArtifactChooseReturnClickCallback;
        }

        protected override SerializableLevelControllerPart GetSerializable()
        {
            return new SerializableLevelBlueprintChooseController()
            {
            };
        }
        public override void LoadFromSerializable(SerializableLevelControllerPart seri)
        {
        }

        public void ApplyChoose()
        {
            ClearChosenBlueprintControllers();
            // 如果根本没有进行选卡，那就不进行替换。
            if (choosingBlueprints == null)
                return;
            for (int i = 0; i < Level.GetSeedSlotCount(); i++)
            {
                NamespaceID blueprintID = null;
                if (i < chosenBlueprints.Count)
                {
                    var index = chosenBlueprints[i];
                    blueprintID = choosingBlueprints[index];
                }
                Level.ReplaceSeedPackAt(i, blueprintID);
            }
            chosenBlueprints.Clear();
            choosingBlueprints = null;
            // 设置制品。
            Level.ReplaceArtifacts(chosenArtifacts);
            chosenArtifacts = null;
        }
        public bool IsInteractable()
        {
            return isChoosingBlueprints && !isViewingLawn;
        }

        #region 选择蓝图
        private void CreateChosenBlueprintController(Blueprint blueprint, int index, SeedDefinition seedDef)
        {
            var controller = new ChosenBlueprintController(Controller, blueprint, index, seedDef);
            controller.Init();
            chosenBlueprintControllers.Add(controller);
        }
        private bool RemoveChosenBlueprintController(int index)
        {
            var controller = chosenBlueprintControllers[index];
            if (controller == null)
                return false;
            controller.Remove();
            chooseUI.RemoveChosenBlueprintAt(index);
            chosenBlueprintControllers.Remove(controller);
            for (int i = index; i < chosenBlueprintControllers.Count; i++)
            {
                chosenBlueprintControllers[i].Index--;
            }
            return true;
        }
        private void ClearChosenBlueprintControllers()
        {
            for (int i = chosenBlueprintControllers.Count - 1; i >= 0; i--)
            {
                var controller = chosenBlueprintControllers[i];
                controller.Destroy();
            }
            chosenBlueprintControllers.Clear();
        }
        public void ChooseBlueprint(int index)
        {
            var seedSlots = Level.GetSeedSlotCount();
            var seedPackIndex = chosenBlueprints.Count;
            if (seedPackIndex >= seedSlots)
                return;
            if (chosenBlueprints.Contains(index))
                return;
            chosenBlueprints.Add(index);

            // 更新UI。
            UpdateBlueprintChooseItem(index);

            // 更新蓝图贴图。
            var seedID = choosingBlueprints[index];
            var seedDef = Game.GetSeedDefinition(seedID);
            var blueprint = chooseUI.CreateChosenBlueprint();
            CreateChosenBlueprintController(blueprint, seedPackIndex, seedDef);

            // 将蓝图移动到目标位置。
            var choosingBlueprintItem = chooseUI.GetBlueprintChooseItem(index);
            blueprint.transform.position = choosingBlueprintItem.transform.position;
            var startPos = blueprint.transform.position;
            var targetPos = chooseUI.GetChosenBlueprintPosition(seedPackIndex);
            var movingBlueprint = chooseUI.CreateMovingBlueprint();
            movingBlueprint.transform.position = startPos;
            movingBlueprint.SetBlueprint(blueprint);
            movingBlueprint.SetMotion(startPos, targetPos);
            movingBlueprint.OnMotionFinished += () =>
            {
                chooseUI.InsertChosenBlueprint(seedPackIndex, blueprint);
                chooseUI.RemoveMovingBlueprint(movingBlueprint);
            };

            // 播放音效。
            Level.PlaySound(VanillaSoundID.tap);
        }
        public void UnchooseBlueprint(int index)
        {
            var choosingIndex = chosenBlueprints[index];
            chosenBlueprints.RemoveAt(index);

            var blueprintUI = chooseUI.GetChosenBlueprintAt(index);
            RemoveChosenBlueprintController(index);

            var startPos = blueprintUI.transform.position;
            var targetBlueprint = chooseUI.GetBlueprintChooseItem(choosingIndex);
            var movingBlueprint = chooseUI.CreateMovingBlueprint();
            movingBlueprint.transform.position = startPos;
            movingBlueprint.SetBlueprint(blueprintUI);
            movingBlueprint.SetMotion(startPos, targetBlueprint.transform);
            movingBlueprint.OnMotionFinished += () =>
            {
                UpdateBlueprintChooseItem(choosingIndex);
                chooseUI.RemoveMovingBlueprint(movingBlueprint);
            };

            // 右侧的可选蓝图全部左移。
            chooseUI.AlignRemainChosenBlueprint(index);

            Level.PlaySound(VanillaSoundID.tap);
            Controller.HideTooltip();
        }
        public void ShowBlueprintChoosePanel(IEnumerable<NamespaceID> blueprints)
        {
            chooseUI.SetSideUIBlend(0);
            chooseUI.SetBlueprintChooseBlend(0);

            isChoosingBlueprints = true;

            // 制品。
            InheritChosenArtifacts();

            Refresh(blueprints);

            // 边缘UI。
            chooseUI.SetSideUIDisplaying(true);
            chooseUI.SetBlueprintChooseDisplaying(true);
        }
        public void Refresh(IEnumerable<NamespaceID> blueprints)
        {
            RefreshChosenArtifacts();
            RefreshBlueprintChoosePanel(blueprints);
        }
        private void RefreshBlueprintChoosePanel(IEnumerable<NamespaceID> blueprints)
        {
            // 保存之前的选卡ID。
            var chosenBlueprintID = chosenBlueprints.Select(i => choosingBlueprints[i]).ToArray();

            // 更新可选蓝图。
            var panelViewData = new BlueprintChoosePanelViewData()
            {
                canViewLawn = Level.CurrentFlag > 0,
                hasCommandBlock = false,
            };
            var orderedBlueprints = new List<NamespaceID>();
            Main.AlmanacManager.GetOrderedBlueprints(blueprints, orderedBlueprints);
            var blueprintViewDatas = orderedBlueprints.Select(id => Main.AlmanacManager.GetChoosingBlueprintViewData(id)).ToArray();
            choosingBlueprints = orderedBlueprints.ToArray();

            // 重新计算选卡映射。
            chosenBlueprints.Clear();
            chosenBlueprints.AddRange(chosenBlueprintID.Select(id => Array.IndexOf(choosingBlueprints, id)));

            // 更新UI。
            chooseUI.SetBlueprintChooseViewAlmanacButtonActive(Main.SaveManager.IsAlmanacUnlocked());
            chooseUI.SetBlueprintChooseViewStoreButtonActive(Main.SaveManager.IsStoreUnlocked());
            chooseUI.UpdateBlueprintChooseElements(panelViewData);
            chooseUI.UpdateBlueprintChooseItems(blueprintViewDatas);
            for (int i = 0; i < orderedBlueprints.Count; i++)
            {
                UpdateBlueprintChooseItem(i);
            }
            UpdateChosenBlueprints();

            // 如果有丢失的卡牌，取消他们的选择。
            for (int i = chosenBlueprints.Count - 1; i >= 0; i--)
            {
                if (chosenBlueprints[i] >= 0)
                    continue;
                UnchooseBlueprint(i);
            }
        }
        private void UpdateChosenBlueprints()
        {
            var slotCount = Level.GetSeedSlotCount();
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
                        viewData = Main.ResourceManager.GetBlueprintViewData(seedDef);
                    }
                }
                var blueprintUI = chooseUI.GetChosenBlueprintAt(i);
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
            var blueprintChooseItem = chooseUI.GetBlueprintChooseItem(index);
            bool selected = chosenBlueprints.Contains(index);
            var id = choosingBlueprints[index];
            bool notRecommended = Level.IsBlueprintNotRecommmended(id);

            blueprintChooseItem.SetDisabled(selected);
            blueprintChooseItem.SetRecharge((selected || notRecommended) ? 1 : 0);
        }
        public void DestroyChosenBlueprintUIAt(int index)
        {
            chooseUI.DestroyChosenBlueprintAt(index);
        }
        private void ShowBlueprintTooltip(NamespaceID blueprintID, ITooltipTarget target)
        {
            var name = GetBlueprintName(blueprintID);
            var tooltip = GetBlueprintTooltip(blueprintID);
            var error = GetBlueprintTooltipError(blueprintID);
            var viewData = new TooltipViewData()
            {
                name = name,
                error = error,
                description = tooltip
            };
            Controller.ShowTooltipOnComponent(target, viewData);
        }
        public string GetBlueprintName(NamespaceID blueprintID)
        {
            var definition = Game.GetSeedDefinition(blueprintID);
            if (definition == null)
                return string.Empty;
            var seedType = definition.GetSeedType();
            if (seedType == SeedTypes.ENTITY)
            {
                var entityID = definition.GetSeedEntityID();
                return Main.ResourceManager.GetEntityName(entityID);
            }
            else if (seedType == SeedTypes.OPTION)
            {
                var optionID = definition.GetSeedOptionID();
                return Main.ResourceManager.GetSeedOptionName(optionID);
            }
            return string.Empty;
        }
        public string GetBlueprintTooltip(NamespaceID blueprintID)
        {
            var definition = Game.GetSeedDefinition(blueprintID);
            if (definition == null)
                return string.Empty;
            var seedType = definition.GetSeedType();
            if (seedType == SeedTypes.ENTITY)
            {
                var entityID = definition.GetSeedEntityID();
                return Main.ResourceManager.GetEntityTooltip(entityID);
            }
            return string.Empty;
        }
        public bool GetBlueprintTooltipError(NamespaceID blueprintID, out string errorMessage)
        {
            errorMessage = null;
            var level = Controller.GetEngine();
            if (level.IsBlueprintNotRecommmended(blueprintID))
            {
                errorMessage = VanillaStrings.NOT_RECOMMONEDED_IN_LEVEL;
                return true;
            }
            return false;
        }
        public string GetBlueprintTooltipError(NamespaceID blueprintID)
        {
            if (GetBlueprintTooltipError(blueprintID, out var errorMessage) && !string.IsNullOrEmpty(errorMessage))
            {
                return Main.LanguageManager._(errorMessage);
            }
            return string.Empty;
        }
        private bool CanChooseBlueprint(NamespaceID id)
        {
            return true;
        }
        #endregion

        #region 制品
        private void RefreshChosenArtifacts()
        {
            var hasArtifacts = Main.SaveManager.GetUnlockedArtifacts().Length > 0;
            chooseUI.SetArtifactSlotsActive(hasArtifacts);

            int artifactCount = Level.GetArtifactSlotCount();
            chooseUI.ResetArtifactSlotCount(artifactCount);

            if (chosenArtifacts == null || artifactCount != chosenArtifacts.Length)
            {
                RemapChosenArtifacts(artifactCount);
            }

            for (int i = 0; i < chosenArtifacts.Length; i++)
            {
                var artifactID = chosenArtifacts[i];
                var sprite = GetArtifactIcon(artifactID);
                var artifactViewData = new ArtifactViewData()
                {
                    sprite = sprite,
                };
                chooseUI.UpdateArtifactSlotAt(i, artifactViewData);
            }
        }
        private void OpenChooseArtifactDialog(int slotIndex)
        {
            choosingArtifactSlotIndex = slotIndex;
            choosingArtifacts = Main.SaveManager.GetUnlockedArtifacts();
            var viewDatas = choosingArtifacts.Select(id =>
            {
                var sprite = GetArtifactIcon(id);
                var disabled = !CanChooseArtifact(id);
                return new ArtifactSelectItemViewData()
                {
                    icon = sprite,
                    selected = chosenArtifacts.Contains(id),
                    disabled = disabled
                };
            }).ToArray();
            chooseUI.ShowArtifactChoosePanel(viewDatas);
            Main.SoundManager.Play2D(VanillaSoundID.tap);
        }
        private void ChooseArtifact(int index)
        {
            var id = choosingArtifacts[index];
            if (!CanChooseArtifact(id))
                return;
            bool isCancel = chosenArtifacts[choosingArtifactSlotIndex] == id;
            for (int i = 0; i < chosenArtifacts.Length; i++)
            {
                if (chosenArtifacts[i] == id)
                {
                    chosenArtifacts[i] = null;
                    SetChosenArtifact(i, null);
                }
            }
            SetChosenArtifact(choosingArtifactSlotIndex, isCancel ? null : id);
            Controller.HideTooltip();
            CloseArtifactChoosePanel();
            Main.SoundManager.Play2D(VanillaSoundID.tap);
        }
        private void InheritChosenArtifacts()
        {
            int artifactCount = Level.GetArtifactSlotCount();
            chosenArtifacts = new NamespaceID[artifactCount];
            for (int i = 0; i < chosenArtifacts.Length; i++)
            {
                var artifact = Level.GetArtifactAt(i);
                var def = artifact?.Definition;
                chosenArtifacts[i] = def?.GetID();
            }
        }
        private void RemapChosenArtifacts(int count)
        {
            var newArray = new NamespaceID[count];
            if (chosenArtifacts != null)
            {
                var max = Mathf.Min(chosenArtifacts.Length, count);
                for (int i = 0; i < max; i++)
                {
                    newArray[i] = chosenArtifacts[i];
                }
            }
            chosenArtifacts = newArray;
        }
        private Sprite GetArtifactIcon(NamespaceID id)
        {
            if (id == null)
                return null;
            var def = Game.GetArtifactDefinition(id);
            return GetArtifactIcon(def);
        }
        private Sprite GetArtifactIcon(ArtifactDefinition def)
        {
            if (def == null)
                return null;
            var spriteRef = def.GetSpriteReference();
            return Main.GetFinalSprite(spriteRef);
        }
        private bool CanChooseArtifact(NamespaceID id)
        {
            return true;
        }
        private void CloseArtifactChoosePanel()
        {
            chooseUI.HideArtifactChoosePanel();
            choosingArtifactSlotIndex = -1;
            choosingArtifacts = null;
        }
        private void SetChosenArtifact(int index, NamespaceID id)
        {
            chosenArtifacts[index] = id;
            var sprite = GetArtifactIcon(id);
            var artifactViewData = new ArtifactViewData()
            {
                sprite = sprite,
            };
            chooseUI.UpdateArtifactSlotAt(index, artifactViewData);
        }
        private void ShowArtifactTooltip(NamespaceID artifactID, ITooltipTarget target)
        {
            var name = Main.ResourceManager.GetArtifactName(artifactID);
            var tooltip = Main.ResourceManager.GetArtifactTooltip(artifactID);
            string error = string.Empty;
            var viewData = new TooltipViewData()
            {
                name = name,
                error = error,
                description = tooltip
            };
            Controller.ShowTooltipOnComponent(target, viewData);
        }
        private void ShowArtifactSlotTooltip(NamespaceID artifactID, ITooltipTarget target)
        {
            TooltipViewData viewData;
            if (NamespaceID.IsValid(artifactID))
            {
                var name = Main.ResourceManager.GetArtifactName(artifactID);
                var tooltip = Main.ResourceManager.GetArtifactTooltip(artifactID);
                viewData = new TooltipViewData()
                {
                    name = name,
                    error = string.Empty,
                    description = tooltip
                };
            }
            else
            {
                viewData = new TooltipViewData()
                {
                    name = Main.LanguageManager._(CHOOSE_ARTIFACT),
                    error = string.Empty,
                    description = string.Empty
                };
            }
            Controller.ShowTooltipOnComponent(target, viewData);
        }
        #endregion

        #region UI层

        #region 事件回调

        #region 可选蓝图
        private void UI_OnBlueprintPointerEnterCallback(int index, PointerEventData eventData)
        {
            ShowBlueprintTooltip(choosingBlueprints[index], chooseUI.GetBlueprintChooseItem(index));
        }
        private void UI_OnBlueprintPointerExitCallback(int index, PointerEventData eventData)
        {
            Controller.HideTooltip();
        }
        private void UI_OnBlueprintPointerDownCallback(int index, PointerEventData eventData)
        {
            var id = choosingBlueprints[index];
            if (!CanChooseBlueprint(id))
                return;
            ChooseBlueprint(index);
        }
        #endregion

        #region 制品选择对话框
        private void UI_OnArtifactChooseItemPointerEnterCallback(int index)
        {
            ShowArtifactTooltip(choosingArtifacts[index], chooseUI.GetArtifactSelectItem(index));
        }
        private void UI_OnArtifactChooseItemPointerExitCallback(int index)
        {
            Controller.HideTooltip();
        }
        private void UI_OnArtifactChooseItemClickCallback(int index)
        {
            ChooseArtifact(index);
        }
        private void UI_OnArtifactChooseReturnClickCallback()
        {
            CloseArtifactChoosePanel();
        }
        #endregion

        #region 界面元素
        private async void UI_OnStartClickCallback()
        {
            List<string> warnings = new List<string>();
            
            if (chosenBlueprints.Count < Level.GetSeedSlotCount())
            {
                warnings.Add(Main.LanguageManager._(WARNING_SELECTED_BLUEPRINTS_NOT_FULL));
            }
            NamespaceID[] blueprintsForChoose = choosingBlueprints.Where(i => CanChooseBlueprint(i)).ToArray();
            NamespaceID[] chosenBlueprintID = chosenBlueprints.Select(i => choosingBlueprints[i]).ToArray();
            Game.RunCallback(LogicLevelCallbacks.GET_BLUEPRINT_WARNINGS, c => c(Level, blueprintsForChoose, chosenBlueprintID, warnings));
            foreach (var warning in warnings)
            {
                var title = Main.LanguageManager._(VanillaStrings.WARNING);
                var desc = warning;
                var result = await Main.Scene.ShowDialogSelectAsync(title, desc);
                if (!result)
                    return;
            }
            isChoosingBlueprints = false;
            StartCoroutine(BlueprintChosenTransition());
        }
        private void UI_OnViewLawnClickCallback()
        {
            isViewingLawn = true;
            viewLawnFinished = false;
            StartCoroutine(BlueprintChooseViewLawnTransition());
        }
        private void UI_OnViewLawnReturnClickCallback()
        {
            viewLawnFinished = true;
        }
        private void UI_OnCommandBlockClickCallback()
        {

        }
        private void UI_OnViewAlmanacClickCallback()
        {
            Controller.OpenAlmanac();
        }
        private void UI_OnViewStoreClickCallback()
        {
            Controller.OpenStore();
        }
        #endregion

        #region 制品槽
        private void UI_OnArtifactSlotPointerEnterCallback(int index)
        {
            ShowArtifactSlotTooltip(chosenArtifacts[index], chooseUI.GetArtifactSlotAt(index));
        }
        private void UI_OnArtifactSlotPointerExitCallback(int index)
        {
            Controller.HideTooltip();
        }
        private void UI_OnArtifactSlotClickCallback(int index)
        {
            OpenChooseArtifactDialog(index);
        }
        #endregion

        #endregion

        #endregion

        private IEnumerator BlueprintChosenTransition()
        {
            chooseUI.SetBlueprintChooseDisplaying(false);
            UI.SetReceiveRaycasts(false);

            yield return new WaitForSeconds(1);
            yield return Controller.GameStartToLawnTransition();
        }
        private IEnumerator BlueprintChooseViewLawnTransition()
        {
            chooseUI.SetBlueprintChooseDisplaying(false);
            UI.SetReceiveRaycasts(false);

            yield return new WaitForSeconds(1);
            yield return Controller.MoveCameraToLawn();
            UI.SetReceiveRaycasts(true);
            chooseUI.SetViewLawnReturnBlockerActive(true);
            Level.ShowAdvice(VanillaStrings.CONTEXT_ADVICE, VanillaStrings.ADVICE_CLICK_TO_CONTINUE, 1000, -1);
            while (!viewLawnFinished)
            {
                yield return null;
            }
            chooseUI.SetViewLawnReturnBlockerActive(false);
            Level.HideAdvice();
            yield return Controller.MoveCameraToChoose();
            chooseUI.SetBlueprintChooseDisplaying(true);
            isViewingLawn = false;
            viewLawnFinished = false;
        }

        [TranslateMsg("对话框内容")]
        public const string WARNING_SELECTED_BLUEPRINTS_NOT_FULL = "你没有携带满蓝图，确认要继续吗？";
        [TranslateMsg("关卡UI")]
        public const string CHOOSE_ARTIFACT = "选择制品";
        private ILevelBlueprintChooseUI chooseUI => UI.BlueprintChoose;

        private bool isChoosingBlueprints;
        private List<int> chosenBlueprints = new List<int>();
        private NamespaceID[] choosingBlueprints;
        private List<ChosenBlueprintController> chosenBlueprintControllers = new List<ChosenBlueprintController>();

        private NamespaceID[] chosenArtifacts;
        private int choosingArtifactSlotIndex;
        private NamespaceID[] choosingArtifacts;

        private bool isViewingLawn;
        private bool viewLawnFinished;
    }
    [Serializable]
    public class SerializableLevelBlueprintChooseController : SerializableLevelControllerPart
    {
    }
    public interface ILevelBlueprintChooseUI
    {
        event Action<int, PointerEventData> OnBlueprintItemPointerEnter;
        event Action<int, PointerEventData> OnBlueprintItemPointerExit;
        event Action<int, PointerEventData> OnBlueprintItemPointerDown;

        event Action OnCommandBlockClick;
        event Action OnStartClick;
        event Action OnViewLawnClick;
        event Action OnViewAlmanacClick;
        event Action OnViewStoreClick;
        event Action OnViewLawnReturnClick;

        event Action<int> OnArtifactSlotClick;
        event Action<int> OnArtifactSlotPointerEnter;
        event Action<int> OnArtifactSlotPointerExit;
        event Action<int> OnArtifactChoosingItemClicked;
        event Action<int> OnArtifactChoosingItemEnter;
        event Action<int> OnArtifactChoosingItemExit;
        event Action OnArtifactChoosingBackClicked;

        void SetSideUIDisplaying(bool displaying);
        void SetBlueprintChooseDisplaying(bool visible);
        void SetSideUIBlend(float blend);
        void SetBlueprintChooseBlend(float blend);


        void SetBlueprintChooseViewAlmanacButtonActive(bool active);
        void SetBlueprintChooseViewStoreButtonActive(bool active);
        void UpdateBlueprintChooseElements(BlueprintChoosePanelViewData viewData);


        void UpdateBlueprintChooseItems(ChoosingBlueprintViewData[] viewDatas);
        Blueprint GetBlueprintChooseItem(int index);



        Blueprint CreateChosenBlueprint();
        void InsertChosenBlueprint(int index, Blueprint blueprint);
        void DestroyChosenBlueprintAt(int index);
        void RemoveChosenBlueprintAt(int index);
        Blueprint GetChosenBlueprintAt(int index);
        Vector3 GetChosenBlueprintPosition(int index);
        void AlignRemainChosenBlueprint(int removeIndex);


        MovingBlueprint CreateMovingBlueprint();
        void RemoveMovingBlueprint(MovingBlueprint blueprint);


        void SetViewLawnReturnBlockerActive(bool active);



        void SetArtifactSlotsActive(bool visible);
        void ShowArtifactChoosePanel(ArtifactSelectItemViewData[] viewDatas);
        void UpdateArtifactSlotAt(int index, ArtifactViewData viewData);
        ArtifactSlot GetArtifactSlotAt(int index);
        ArtifactSelectItem GetArtifactSelectItem(int index);
        void HideArtifactChoosePanel();
        void ResetArtifactSlotCount(int count);
    }
}
