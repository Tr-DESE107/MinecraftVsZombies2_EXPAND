using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MVZ2.UI
{
    public class RaycastReciver : MonoBehaviour, IPointerDownHandler
    {
        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            OnPointerDown?.Invoke();
        }
        public event Action OnPointerDown;
    }
}
