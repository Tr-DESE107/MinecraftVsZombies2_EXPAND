using System;
using MVZ2.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MVZ2.Level.UI
{
    public class PickaxeSlot : LevelUIUnit, IPointerDownHandler, IPointerExitHandler, IPointerEnterHandler, ITooltipUI
    {
        public void SetPickaxeVisible(bool visible)
        {
            image.enabled = visible;
        }
        private void Awake()
        {

        }
        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            OnPointerDown?.Invoke(eventData);
        }
        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {
            OnPointerEnter?.Invoke(eventData);
        }
        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {
            OnPointerExit?.Invoke(eventData);
        }

        public event Action<PointerEventData> OnPointerEnter;
        public event Action<PointerEventData> OnPointerExit;
        public event Action<PointerEventData> OnPointerDown;
        TooltipAnchor ITooltipUI.Anchor => tooltipAnchor;
        [SerializeField]
        private Image image;
        [SerializeField]
        private TooltipAnchor tooltipAnchor;
    }
}
