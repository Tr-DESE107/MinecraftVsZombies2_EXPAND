using System;
using MVZ2.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MVZ2.Level.UI
{
    public class PickaxeSlot : LevelUIUnit, IPointerDownHandler
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
        public event Action<PointerEventData> OnPointerDown;
        [SerializeField]
        private Image image;
    }
}
