using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MVZ2.Level.UI
{
    public class StarshardPanelIcon : MonoBehaviour, IPointerDownHandler
    {
        public void SetSprite(Sprite sprite)
        {
            image.sprite = sprite;
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
