using System;
using MVZ2.Level.UI;
using MVZ2.Models;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MVZ2.UI
{
    public class Blueprint : MonoBehaviour, IPointerDownHandler, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, ITooltipTarget
    {
        public void UpdateView(BlueprintViewData viewData)
        {
            SetEmpty(viewData.empty);
            SetCost(viewData.cost);
            SetIcon(viewData.icon);
            SetTriggerActive(viewData.triggerActive);
            foreach (var preset in normalPresets)
            {
                preset.SetActive(viewData.preset == BlueprintPreset.Normal);
            }
            foreach (var preset in upgradePresets)
            {
                preset.SetActive(viewData.preset == BlueprintPreset.Upgrade);
            }
        }
        public void SetEmpty(bool empty)
        {
            emptyObj.SetActive(empty);
            rootObj.SetActive(!empty);
        }
        public void SetIcon(Sprite sprite)
        {
            iconImage.enabled = sprite;
            iconImage.sprite = sprite;
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
        public void SetTwinkling(bool twinkling)
        {
            if (!animator.gameObject.activeInHierarchy)
                return;
            animator.SetBool("Twinkling", twinkling);
        }
        public void PointerRelease()
        {
            OnPointerRelease?.Invoke(this);
        }
        private void Awake()
        {
            animator.logWarnings = false;
        }
        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            OnPointerDown?.Invoke(this, eventData);
        }
        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {
            OnPointerClick?.Invoke(this, eventData);
        }
        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {
            OnPointerEnter?.Invoke(this, eventData);
        }
        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {
            OnPointerExit?.Invoke(this, eventData);
        }
        public event Action<Blueprint, PointerEventData> OnPointerDown;
        public event Action<Blueprint, PointerEventData> OnPointerClick;
        public event Action<Blueprint, PointerEventData> OnPointerEnter;
        public event Action<Blueprint, PointerEventData> OnPointerExit;
        public event Action<Blueprint> OnPointerRelease;
        TooltipAnchor ITooltipTarget.Anchor => tooltipAnchor;
        public UIModel Model => model;
        [SerializeField]
        private Animator animator;
        [SerializeField]
        private GameObject emptyObj;
        [SerializeField]
        private GameObject rootObj;
        [SerializeField]
        private GameObject[] normalPresets;
        [SerializeField]
        private GameObject[] upgradePresets;
        [SerializeField]
        private Image iconImage;
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
        public static readonly BlueprintViewData Empty = new BlueprintViewData { empty = true };
    }
    public enum BlueprintPreset
    {
        Normal,
        Upgrade
    }
}
