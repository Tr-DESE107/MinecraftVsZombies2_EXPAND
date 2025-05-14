using System;
using UnityEngine;

namespace MVZ2.UI
{
    public class CommandBlockSlot : MonoBehaviour
    {
        public void UpdateCommandBlockItem(ChoosingBlueprintViewData viewData)
        {
            var blueprint = commandBlockBlueprint;
            blueprint.UpdateView(viewData.blueprint);
            blueprint.SetDisabled(viewData.disabled || viewData.selected);
            blueprint.SetRecharge(viewData.selected ? 1 : 0);
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
            commandBlockBlueprint.OnPointerEnter += (blueprint, eventData) => OnPointerEnter?.Invoke();
            commandBlockBlueprint.OnPointerExit += (blueprint, eventData) => OnPointerExit?.Invoke();
            commandBlockBlueprint.OnPointerDown += (blueprint, eventData) => OnPointerDown?.Invoke();
            commandBlockBlueprint.OnPointerClick += (blueprint, eventData) => OnClick?.Invoke();
        }
        public event Action OnPointerEnter;
        public event Action OnPointerExit;
        public event Action OnPointerDown;
        public event Action OnClick;
        [SerializeField]
        GameObject commandBlockRoot;
        [SerializeField]
        Blueprint commandBlockBlueprint;
    }
}
