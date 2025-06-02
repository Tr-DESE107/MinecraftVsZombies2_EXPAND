﻿using System;
using MVZ2.UI;
using MVZ2Logic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MVZ2.Level.UI
{
    public class CommandBlockChoosePanel : MonoBehaviour
    {
        public void UpdateItems(ChoosingBlueprintViewData[] viewDatas)
        {
            displayer.UpdateItems(viewDatas);
        }
        public Blueprint GetItem(int index)
        {
            return displayer.GetItem(index);
        }
        private void Awake()
        {
            cancelButton.onClick.AddListener(() => OnCancelButtonClick?.Invoke());
            displayer.OnBlueprintPointerInteraction += (index, data, i) => OnBlueprintPointerInteraction?.Invoke(index, data, i);
            displayer.OnBlueprintSelect += (index) => OnBlueprintSelect?.Invoke(index);
        }
        public event Action OnCancelButtonClick;
        public event Action<int, PointerEventData, PointerInteraction> OnBlueprintPointerInteraction;
        public event Action<int> OnBlueprintSelect;
        [SerializeField]
        Button cancelButton;
        [SerializeField]
        BlueprintDisplayer displayer;
    }
}
