using System;
using MVZ2.Level.UI;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MVZ2.UI
{
    public class Blueprint : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler, ITooltipUI
    {
        public void UpdateView(BlueprintViewData viewData)
        {
            SetEmpty(viewData.empty);
            SetCost(viewData.cost);
            SetIcon(viewData.icon);
            SetTriggerActive(viewData.triggerActive);
            SetTriggerCost(viewData.triggerCost);
        }
        public void SetEmpty(bool empty)
        {
            emptyImageObj.SetActive(empty);
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
        public void SetTriggerCost(string cost)
        {
            triggerCostText.text = cost;
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
            animator.SetBool("Twinkling", twinkling);
        }
        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            OnPointerDown?.Invoke(this, eventData);
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
        public event Action<Blueprint, PointerEventData> OnPointerEnter;
        public event Action<Blueprint, PointerEventData> OnPointerExit;
        TooltipAnchor ITooltipUI.Anchor => tooltipAnchor;
        [SerializeField]
        private Animator animator;
        [SerializeField]
        private GameObject emptyImageObj;
        [SerializeField]
        private GameObject rootObj;
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
        private TextMeshProUGUI triggerCostText;
        [SerializeField]
        private TooltipAnchor tooltipAnchor;

    }
    public struct BlueprintViewData
    {
        public bool empty;
        public string cost;
        public Sprite icon;
        public bool triggerActive;
        public string triggerCost;
    }
}
