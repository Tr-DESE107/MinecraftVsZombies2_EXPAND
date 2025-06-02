﻿using System;
using MVZ2Logic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MVZ2.UI
{
    public abstract class BlueprintDisplayer : MonoBehaviour
    {
        public abstract void UpdateItems(ChoosingBlueprintViewData[] viewDatas);
        public abstract Blueprint GetItem(int index);
        protected void CallBlueprintPointerInteraction(int index, PointerEventData eventData, PointerInteraction interaction)
        {
            OnBlueprintPointerInteraction?.Invoke(index, eventData, interaction);
        }
        protected void CallBlueprintSelect(int index)
        {
            OnBlueprintSelect?.Invoke(index);
        }
        public event Action<int, PointerEventData, PointerInteraction> OnBlueprintPointerInteraction;
        public event Action<int> OnBlueprintSelect;
    }
    public struct ChoosingBlueprintViewData
    {
        public BlueprintViewData blueprint;
        public bool disabled;
        public bool selected;

        public static readonly ChoosingBlueprintViewData Empty = new ChoosingBlueprintViewData()
        {
            blueprint = BlueprintViewData.Empty
        };
    }
}
