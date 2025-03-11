using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MVZ2.Level.UI
{
    public class PickaxeSlot : LevelUIUnit, IPointerDownHandler, IPointerExitHandler, IPointerEnterHandler, ITooltipTarget
    {
        public void SetSelected(bool selected)
        {
            animator.SetBool("Selected", selected);
        }
        public void SetDisabled(bool selected)
        {
            animator.SetBool("Disabled", selected);
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
        TooltipAnchor ITooltipTarget.Anchor => tooltipAnchor;
        [SerializeField]
        private Animator animator;
        [SerializeField]
        private TooltipAnchor tooltipAnchor;
    }
}
