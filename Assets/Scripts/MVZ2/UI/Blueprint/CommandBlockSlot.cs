﻿using System;
using MVZ2Logic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MVZ2.UI
{
    public class CommandBlockSlot : MonoBehaviour
    {
        public void UpdateCommandBlockItem(ChoosingBlueprintViewData viewData)
        {
            var blueprint = commandBlockBlueprint;
            blueprint.UpdateView(viewData.blueprint);
            blueprint.SetDisabled(viewData.disabled);
            blueprint.SetSelected(viewData.selected);
            blueprint.SetRecharge(viewData.recharge);
        }
        public void SetCommandBlockActive(bool value)
        {
            commandBlockRoot.SetActive(value);
        }
        public Blueprint GetCommandBlockBlueprint()
        {
            return commandBlockBlueprint;
        }
        private void Awake()
        {
            commandBlockBlueprint.OnPointerInteraction += (blueprint, eventData, i) => OnPointerInteraction?.Invoke(eventData, i);
            commandBlockBlueprint.OnSelect += (blueprint, data) => OnSelect?.Invoke(data);
        }
        public event Action<PointerEventData, PointerInteraction> OnPointerInteraction;
        public event Action<PointerEventData> OnSelect;
        [SerializeField]
        GameObject commandBlockRoot;
        [SerializeField]
        Blueprint commandBlockBlueprint;
    }
}
