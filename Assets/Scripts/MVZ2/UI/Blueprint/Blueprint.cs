using System;
using MVZ2.HeldItems;
using MVZ2.Level;
using MVZ2.Level.UI;
using MVZ2.Managers;
using MVZ2.Models;
using MVZ2Logic;
using MVZ2Logic.HeldItems;
using PVZEngine.Level;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MVZ2.UI
{
    public class Blueprint : MonoBehaviour, ILevelRaycastReceiver, ITooltipTarget
    {
        public void UpdateView(BlueprintViewData viewData)
        {
            SetEmpty(viewData.empty);
            SetCost(viewData.cost);
            SetTriggerActive(viewData.triggerActive);
            foreach (var preset in normalPresets)
            {
                preset.SetActive(viewData.preset == BlueprintPreset.Normal);
            }
            foreach (var preset in upgradePresets)
            {
                preset.SetActive(viewData.preset == BlueprintPreset.Upgrade);
            }
            foreach (var preset in commandBlockPresets)
            {
                preset.SetActive(viewData.preset == BlueprintPreset.CommandBlock);
            }
            var icon = viewData.icon;
            iconImage.enabled = icon && !viewData.iconGrayscale;
            iconImage.sprite = icon;
            iconImageCommandBlock.enabled = icon && viewData.iconGrayscale;
            iconImageCommandBlock.sprite = icon;
        }
        public void SetEmpty(bool empty)
        {
            emptyObj.SetActive(empty);
            rootObj.SetActive(!empty);
        }
        public void SetCost(string cost)
        {
            costText.text = cost;
        }
        public void SetTriggerActive(bool active)
        {
            triggerCostObject.SetActive(active);
        }
        public void SetRecharge(float charge)
        {
            rechargeImage.fillAmount = charge;
        }
        public void SetDisabled(bool disabled)
        {
            disabledObject.SetActive(disabled);
        }
        public void SetSelected(bool selected)
        {
            selectedObject.SetActive(selected);
        }
        public void SetTwinkleAlpha(float alpha)
        {
            var color = twinkleImage.color;
            color.a = alpha;
            twinkleImage.color = color;
        }
        private void Awake()
        {
            holdStreakHandler.OnPointerInteraction += (_, d, i) => CallPointerInteraction(d, i);
        }
        bool ILevelRaycastReceiver.IsValidReceiver(LevelEngine level, HeldItemDefinition definition, IHeldItemData data, PointerEventData eventData)
        {
            if (definition == null)
                return false;
            if (Index < 0)
                return false;
            var target = new HeldItemTargetBlueprint(level, Index, IsInConveyor);
            var pointer = InputManager.GetPointerDataFromEventData(eventData);
            return definition.IsValidFor(target, data, pointer);
        }
        int ILevelRaycastReceiver.GetSortingLayer()
        {
            var canvas = (transform as RectTransform).GetRootCanvas();
            return canvas.sortingLayerID;
        }
        int ILevelRaycastReceiver.GetSortingOrder()
        {
            var canvas = (transform as RectTransform).GetRootCanvas();
            return canvas.sortingOrder;
        }
        private void CallPointerInteraction(PointerEventData eventData, PointerInteraction interaction)
        {
            OnPointerInteraction?.Invoke(this, eventData, interaction);
            if (interaction == selectInteraction)
            {
                OnSelect?.Invoke(this, eventData);
            }
        }
        public event Action<Blueprint, PointerEventData, PointerInteraction> OnPointerInteraction;
        public event Action<Blueprint, PointerEventData> OnSelect;
        TooltipAnchor ITooltipTarget.Anchor => tooltipAnchor;
        public UIModel Model => model;
        public int Index { get; set; } = -1;
        public bool IsInConveyor { get; set; }
        [SerializeField]
        private PointerInteraction selectInteraction = PointerInteraction.Down;
        [SerializeField]
        private LevelPointerInteractionHandler holdStreakHandler;
        [SerializeField]
        private GameObject emptyObj;
        [SerializeField]
        private GameObject rootObj;
        [SerializeField]
        private GameObject[] normalPresets;
        [SerializeField]
        private GameObject[] upgradePresets;
        [SerializeField]
        protected GameObject[] commandBlockPresets;
        [SerializeField]
        private Image iconImage;
        [SerializeField]
        private Image iconImageCommandBlock;
        [SerializeField]
        private Image twinkleImage;
        [SerializeField]
        private TextMeshProUGUI costText;
        [SerializeField]
        private Image rechargeImage;
        [SerializeField]
        private GameObject selectedObject;
        [SerializeField]
        private GameObject disabledObject;
        [SerializeField]
        private GameObject triggerCostObject;
        [SerializeField]
        private UIModel model;
        [SerializeField]
        private TooltipAnchor tooltipAnchor;

    }
    public struct BlueprintViewData
    {
        public bool empty;
        public string cost;
        public Sprite icon;
        public bool triggerActive;
        public BlueprintPreset preset;
        public bool iconGrayscale;
        public static readonly BlueprintViewData Empty = new BlueprintViewData { empty = true };
    }
    public enum BlueprintPreset
    {
        Normal,
        Upgrade,
        CommandBlock,
    }
}
