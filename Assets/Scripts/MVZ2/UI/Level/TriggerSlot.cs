using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MVZ2.Level.UI
{
    public class TriggerSlot : LevelUIUnit, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, ITooltipTarget
    {
        public void SetSelected(bool selected)
        {
            if (animator.isActiveAndEnabled)
                animator.SetBool("Selected", selected);
        }
        public void SetHotkeyText(string hotkey)
        {
            if (hotkeyText)
                hotkeyText.text = hotkey;
        }
        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {
            OnPointerEnter?.Invoke(eventData);
        }
        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {
            OnPointerExit?.Invoke(eventData);
        }
        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            OnPointerDown?.Invoke(eventData);
        }
        public event Action<PointerEventData> OnPointerEnter;
        public event Action<PointerEventData> OnPointerExit;
        public event Action<PointerEventData> OnPointerDown;
        [SerializeField]
        private Animator animator;
        [SerializeField]
        private TooltipAnchor tooltipAnchor;
        [SerializeField]
        private TextMeshProUGUI hotkeyText;

        TooltipAnchor ITooltipTarget.Anchor => tooltipAnchor;
    }
}
