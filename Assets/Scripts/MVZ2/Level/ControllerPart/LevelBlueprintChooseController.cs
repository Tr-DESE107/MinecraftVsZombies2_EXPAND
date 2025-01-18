using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MukioI18n;
using MVZ2.Level.UI;
using MVZ2.UI;
using MVZ2.Vanilla;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Level;
using MVZ2.Vanilla.Saves;
using MVZ2.Vanilla.SeedPacks;
using MVZ2Logic.Artifacts;
using MVZ2Logic.Callbacks;
using MVZ2Logic.Games;
using MVZ2Logic.Level;
using MVZ2Logic.SeedPacks;
using PVZEngine;
using PVZEngine.Definitions;
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
            chooseUI.OnRepickClick += UI_OnRepickClickCallback;
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

            chooseUI.SetChosenBlueprintsVisible(false);
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
            chosenBlueprintControllers.Insert(index, controller);
            UpdateControllerIndexes();
        }
        private bool RemoveChosenBlueprintController(int index)
        {
            var controller = chosenBlueprintControllers[index];
            if (controller == null)
                return false;
            controller.Remove();
            chooseUI.RemoveChosenBlueprintAt(index);
            chosenBlueprintControllers.Remove(controller);
            UpdateControllerIndexes();
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

        #region 蓝图移动
        public void ChooseBlueprint(int index)
        {
            var seedSlots = Level.GetSeedSlotCount();
            var seedPackIndex = chosenBlueprints.Count;
            if (seedPackIndex >= seedSlots)
                return;
            if (chosenBlueprints.Contains(index))
                return;
            LoadBlueprint(seedPackIndex, index);

            // 播放音效。
            Level.PlaySound(VanillaSoundID.tap);
        }
        public void UnchooseBlueprint(int index)
        {
            UnloadBlueprint(index);

            Level.PlaySound(VanillaSoundID.tap);
            Controller.HideTooltip();
        }
        private void LoadBlueprint(int seedPackIndex, int index)
        {
            chosenBlueprints.Insert(seedPackIndex, index);

            // 更新可选蓝图UI。
            UpdateBlueprintChooseItem(index);

            // 创造已选蓝图UI。
            var seedID = choosingBlueprints[index];
            var seedDef = Game.GetSeedDefinition(seedID);
            var blueprint = chooseUI.CreateChosenBlueprint();

            // 创建已选蓝图控制器。
            CreateChosenBlueprintController(blueprint, seedPackIndex, seedDef);

            // 将蓝图移动到目标位置。
            var choosingBlueprintItem = chooseUI.GetBlueprintChooseItem(index);
            blueprint.transform.position = choosingBlueprintItem.transform.position;
            var startPos = blueprint.transform.position;
            var targetPos = chooseUI.GetChosenBlueprintPosition(seedPackIndex);

            // 加一个占位符
            var placeHolder = chooseUI.CreateChosenBlueprint();
            placeHolder.UpdateView(BlueprintViewData.Empty);
            chooseUI.InsertChosenBlueprint(seedPackIndex, placeHolder);

            var movingBlueprint = chooseUI.CreateMovingBlueprint();
            movingBlueprint.transform.position = startPos;
            movingBlueprint.SetBlueprint(blueprint);
            movingBlueprint.SetMotion(startPos, targetPos);
            movingBlueprint.OnMotionFinished += () =>
            {
                // 移除占位符并替换为该蓝图
                chooseUI.DestroyChosenBlueprint(placeHolder);
                chooseUI.InsertChosenBlueprint(seedPackIndex, blueprint);
                chooseUI.RemoveMovingBlueprint(movingBlueprint);
            };
        }
        private void UnloadBlueprint(int index)
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
        }
        private void MoveChosenBlueprint(int fromIndex, int toIndex)
        {
            if (fromIndex == toIndex)
                return;
            var index = chosenBlueprints[fromIndex];
            var controller = chosenBlueprintControllers[fromIndex];
            var blueprint = chooseUI.GetChosenBlueprintAt(fromIndex);

            chosenBlueprints.RemoveAt(fromIndex);
            chosenBlueprintControllers.RemoveAt(fromIndex);
            chooseUI.RemoveChosenBlueprintAt(fromIndex);

            if (toIndex > chosenBlueprints.Count)
            {
                toIndex = chosenBlueprints.Count;
            }
            chosenBlueprints.Insert(toIndex, index);
            chosenBlueprintControllers.Insert(toIndex, controller);
            chooseUI.InsertChosenBlueprint(toIndex, blueprint);

            UpdateControllerIndexes();
        }
        private void SwapChosenBlueprints(int index1, int index2)
        {
            if (index1 == index2)
                return;
            var min = Mathf.Min(index1, index2);
            var max = Mathf.Max(index1, index2);
            MoveChosenBlueprint(min, max);
            MoveChosenBlueprint(max - 1, min);
        }
        private void UpdateControllerIndexes()
        {
            for (int i = 0; i < chosenBlueprintControllers.Count; i++)
            {
                chosenBlueprintControllers[i].Index = i;
            }
        }
        #endregion

        #region 替换
        public async void ReplaceChoosing(BlueprintSelectionItem[] blueprints, ArtifactSelectionItem[] artifacts)
        {
            if (blueprints != null)
            {
                if (ValidateReplaceBlueprints(blueprints, out var contraptionMessage))
                {
                    var alignedBlueprints = blueprints.Where(i => NamespaceID.IsValid(i.id)).ToArray();
                    ReplaceBlueprints(alignedBlueprints);
                }
                else
                {
                    var title = Main.LanguageManager._(VanillaStrings.ERROR);
                    var desc = Main.LanguageManager._(contraptionMessage);
                    await Main.Scene.ShowDialogMessageAsync(title, desc);
                }
            }
            if (artifacts != null)
            {
                if (ValidateReplaceArtifacts(artifacts, out var artifactMessage))
                {
                    var alignedArtifacts = artifacts.Where(i => NamespaceID.IsValid(i.id)).ToArray();
                    ReplaceArtifacts(alignedArtifacts);
                }
                else
                {
                    var title = Main.LanguageManager._(VanillaStrings.ERROR);
                    var desc = Main.LanguageManager._(artifactMessage);
                    await Main.Scene.ShowDialogMessageAsync(title, desc);
                }
            }
        }
        private bool ValidateReplaceBlueprints(BlueprintSelectionItem[] blueprints, out string errorMessage)
        {
            if (blueprints.GroupBy(x => x).Any(g => g.Count() > 1))
            {
                errorMessage = REPLACE_ERROR_DUPLICATE_BLUEPRINTS;
                return false;
            }
            if (blueprints.Any(item => !Main.SaveManager.IsContraptionUnlocked(item.id)))
            {
                errorMessage = REPLACE_ERROR_CONTRAPTION_LOCKED;
                return false;
            }
            errorMessage = null;
            return true;
        }
        private bool ValidateReplaceArtifacts(ArtifactSelectionItem[] artifacts, out string errorMessage)
        {
            if (artifacts.GroupBy(x => x).Any(g => g.Count() > 1))
            {
                errorMessage = REPLACE_ERROR_DUPLICATE_ARTIFACTS;
                return false;
            }
            if (artifacts.Any(item => !Main.SaveManager.IsArtifactUnlocked(item.id)))
            {
                errorMessage = REPLACE_ERROR_ARTIFACT_LOCKED;
                return false;
            }
            errorMessage = null;
            return true;
        }
        private void ReplaceBlueprints(BlueprintSelectionItem[] blueprints)
        {
            var chosenID = chosenBlueprints.Select(i => choosingBlueprints[i]);
            var retainBlueprints = blueprints.Where(item => chosenID.Contains(item.id)).ToArray();
            var pickBlueprints = blueprints.Except(retainBlueprints).ToArray();
            var removeBlueprints = chosenID.Where(id => !blueprints.Any(item => item.id == id)).ToArray();

            // 首先卸下要移除的蓝图。
            foreach (var id in removeBlueprints)
            {
                var existingIndex = chosenBlueprints.FindIndex(i => choosingBlueprints[i] == id);
                UnloadBlueprint(existingIndex);
            }
            // 然后装载要添加的蓝图。
            foreach (var item in pickBlueprints)
            {
                var id = item.id;
                var targetIndex = Array.IndexOf(blueprints, item);
                var choosingIndex = Array.IndexOf(choosingBlueprints, id);
                LoadBlueprint(targetIndex, choosingIndex);
            }
            // 最后将要保留的蓝图交换位置。
            foreach (var item in retainBlueprints)
            {
                var id = item.id;
                var targetIndex = Array.IndexOf(blueprints, item);
                var existingIndex = chosenBlueprints.FindIndex(i => choosingBlueprints[i] == id);
                SwapChosenBlueprints(existingIndex, targetIndex);
            }
        }
        private void ReplaceArtifacts(ArtifactSelectionItem[] artifactsID)
        {
            for (int i = 0; i < chosenArtifacts.Length; i++)
            {
                if (i < artifactsID.Length)
                {
                    SetChosenArtifact(i, artifactsID[i]?.id);
                }
                else
                {
                    SetChosenArtifact(i, null);
                }
            }
        }
        #endregion

        public void ShowBlueprintChoosePanel(IEnumerable<NamespaceID> blueprints)
        {
            chooseUI.SetSideUIBlend(0);
            chooseUI.SetBlueprintChooseBlend(0);

            isChoosingBlueprints = true;

            chooseUI.SetChosenBlueprintsVisible(true);
            // 制品。
            InheritChosenArtifacts();

            Refresh(blueprints);

            // 边缘UI。
            chooseUI.SetSideUIDisplaying(true);
            chooseUI.SetBlueprintChooseDisplaying(true);
        }
        public void Refresh(IEnumerable<NamespaceID> blueprints)
        {
            chooseUI.SetChosenBlueprintsSlotCount(Level.GetSeedSlotCount());
            RefreshChosenArtifacts();
            RefreshBlueprintChoosePanel(blueprints);
        }
        private void RefreshBlueprintChoosePanel(IEnumerable<NamespaceID> blueprints)
        {
            // 保存之前的选卡ID。
            var chosenBlueprintID = chosenBlueprints.Select(i => choosingBlueprints[i]).ToArray();

            // 更新可选蓝图。
            bool canRepick = Main.SaveManager.GetLastSelection() != null;
            var panelViewData = new BlueprintChoosePanelViewData()
            {
                canViewLawn = Level.CurrentFlag > 0,
                hasCommandBlock = false,
                canRepick = canRepick,
            };
            var orderedBlueprints = new List<NamespaceID>();
            Main.AlmanacManager.GetOrderedBlueprints(blueprints, orderedBlueprints);
            var blueprintViewDatas = orderedBlueprints.Select(id => Main.AlmanacManager.GetChoosingBlueprintViewData(id, Level.IsEndless())).ToArray();
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
                        viewData = Main.ResourceManager.GetBlueprintViewData(seedDef, Level.IsEndless());
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

            var selectionBlueprints = chosenBlueprintID.Select(id => new BlueprintSelectionItem() { id = id, isCommandBlock = false }).ToArray();
            var selection = new BlueprintSelection()
            {
                blueprints = selectionBlueprints,
                artifacts = chosenArtifacts.Where(e => NamespaceID.IsValid(e)).Select(id => new ArtifactSelectionItem() { id = id }).ToArray()
            };
            Main.SaveManager.SetLastSelection(selection);

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
        private void UI_OnRepickClickCallback()
        {
            var lastSelection = Main.SaveManager.GetLastSelection();
            if (lastSelection == null)
                return;
            var blueprints = lastSelection.blueprints;
            var artifacts = lastSelection.artifacts;
            ReplaceChoosing(blueprints, artifacts);
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
        [TranslateMsg("重选蓝图的验证错误信息")]
        public const string REPLACE_ERROR_DUPLICATE_BLUEPRINTS = "上一次选择的蓝图中包含多个相同的器械。";
        [TranslateMsg("重选蓝图的验证错误信息")]
        public const string REPLACE_ERROR_CONTRAPTION_LOCKED = "上一次选择的蓝图中包含未解锁的器械。";
        [TranslateMsg("重选蓝图的验证错误信息")]
        public const string REPLACE_ERROR_DUPLICATE_ARTIFACTS = "上一次选择的制品中包含多个相同的制品。";
        [TranslateMsg("重选蓝图的验证错误信息")]
        public const string REPLACE_ERROR_ARTIFACT_LOCKED = "上一次选择的制品中包含未解锁的制品。";
        private LevelUIBlueprintChoose chooseUI => UI.BlueprintChoose;

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
}
