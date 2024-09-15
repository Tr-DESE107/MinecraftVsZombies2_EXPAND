using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MVZ2.Level.UI
{
    public class TriggerSlot : LevelUIUnit, IPointerDownHandler
    {
        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            OnPointerDown?.Invoke(eventData);
        }
        public event Action<PointerEventData> OnPointerDown;
        [SerializeField]
        private Image image;
    }
}
