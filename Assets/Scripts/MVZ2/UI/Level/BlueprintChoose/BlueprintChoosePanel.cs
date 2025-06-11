﻿using System;
using MVZ2.UI;
using MVZ2Logic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MVZ2.Level.UI
{
    public class BlueprintChoosePanel : MonoBehaviour
    {
        public void UpdateElements(BlueprintChoosePanelViewData viewData)
        {
            viewLawnButton.gameObject.SetActive(viewData.canViewLawn);
            repickButton.gameObject.SetActive(viewData.canRepick);
            commandBlockSlot.SetCommandBlockActive(viewData.hasCommandBlock);
        }
        public void UpdateCommandBlockItem(ChoosingBlueprintViewData viewData)
        {
            commandBlockSlot.UpdateCommandBlockItem(viewData);
        }
        public void UpdateItems(ChoosingBlueprintViewData[] viewDatas)
        {
            displayer.UpdateItems(viewDatas);
        }
        public Blueprint GetItem(int index)
        {
            return displayer.GetItem(index);
        }
        public Blueprint GetCommandBlockBlueprintItem()
        {
            return commandBlockSlot.GetCommandBlockBlueprint();
        }
        private void Awake()
        {
            startButton.onClick.AddListener(() => OnStartButtonClick?.Invoke());
            viewLawnButton.onClick.AddListener(() => OnViewLawnButtonClick?.Invoke());
            cancelButton.onClick.AddListener(() => OnCancelButtonClick?.Invoke());
            repickButton.onClick.AddListener(() => OnRepickButtonClick?.Invoke());
            displayer.OnBlueprintPointerInteraction += (index, data, i) => OnBlueprintPointerInteraction?.Invoke(index, data, i);
            displayer.OnBlueprintSelect += (index, data) => OnBlueprintSelect?.Invoke(index, data);
            commandBlockSlot.OnPointerInteraction += (e, i) => OnCommandBlockBlueprintPointerInteraction?.Invoke(e, i);
            commandBlockSlot.OnSelect += (data) => OnCommandBlockBlueprintSelect?.Invoke(data);
        }
        protected void CallBlueprintPointerInteraction(int index, PointerEventData eventData, PointerInteraction interaction)
        {
            OnBlueprintPointerInteraction?.Invoke(index, eventData, interaction);
        }
        public event Action OnStartButtonClick;
        public event Action OnViewLawnButtonClick;
        public event Action OnCancelButtonClick;
        public event Action OnRepickButtonClick;
        public event Action<PointerEventData, PointerInteraction> OnCommandBlockBlueprintPointerInteraction;
        public event Action<PointerEventData> OnCommandBlockBlueprintSelect;
        public event Action<int, PointerEventData, PointerInteraction> OnBlueprintPointerInteraction;
        public event Action<int, PointerEventData> OnBlueprintSelect;
        [SerializeField]
        Button startButton;
        [SerializeField]
        Button viewLawnButton;
        [SerializeField]
        Button cancelButton;
        [SerializeField]
        Button repickButton;
        [SerializeField]
        BlueprintDisplayer displayer;
        [SerializeField]
        CommandBlockSlot commandBlockSlot;
    }
    public struct BlueprintChoosePanelViewData
    {
        public bool canViewLawn;
        public bool hasCommandBlock;
        public bool canRepick;
    }
}
