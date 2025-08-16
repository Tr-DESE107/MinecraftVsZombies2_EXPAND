using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MVZ2.UI
{
    public class TooltipHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ITooltipTarget
    {
        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {
            OnPointerEnter?.Invoke(this);
        }
        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {
            OnPointerExit?.Invoke(this);
        }
        public event Action<TooltipHandler> OnPointerEnter;
        public event Action<TooltipHandler> OnPointerExit;

        public ITooltipAnchor Anchor => anchor;
        public TooltipAnchor anchor;
        public string text;

    }
}
