using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MVZ2.Map
{
    public class MapRaycastReceiver : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IScrollHandler
    {
        void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
        {
            OnBeginDrag?.Invoke(eventData);
        }

        void IDragHandler.OnDrag(PointerEventData eventData)
        {
            OnDrag?.Invoke(eventData);
        }

        void IEndDragHandler.OnEndDrag(PointerEventData eventData)
        {
            OnEndDrag?.Invoke(eventData);
        }

        void IScrollHandler.OnScroll(PointerEventData eventData)
        {
            OnScroll?.Invoke(eventData);
        }
        public event Action<PointerEventData> OnBeginDrag;
        public event Action<PointerEventData> OnDrag;
        public event Action<PointerEventData> OnEndDrag;
        public event Action<PointerEventData> OnScroll;
    }
}
