using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MVZ2.UI
{
    public class Blueprint : MonoBehaviour, IPointerDownHandler
    {
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
        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            OnPointerDown?.Invoke();
        }
        public event Action OnPointerDown;
        [SerializeField]
        private Image iconImage;
        [SerializeField]
        private TextMeshProUGUI costText;
        [SerializeField]
        private GameObject triggerCostObject;
        [SerializeField]
        private TextMeshProUGUI triggerCostText;

    }
    public struct BlueprintViewData
    {
        public string cost;
        public Sprite icon;
        public bool triggerActive;
        public string triggerCost;
    }
}
