using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MVZ2
{
    public class GridController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        public void SetColor(Color color)
        {
            spriteRenderer.color = color;
        }
        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {
            OnPointerEnter?.Invoke(eventData);
        }
        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {
            OnPointerExit?.Invoke(eventData);
        }
        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {
            OnPointerClick?.Invoke(eventData);
        }
        public event Action<PointerEventData> OnPointerEnter;
        public event Action<PointerEventData> OnPointerExit;
        public event Action<PointerEventData> OnPointerClick;
        [SerializeField]
        private SpriteRenderer spriteRenderer;
    }
}
