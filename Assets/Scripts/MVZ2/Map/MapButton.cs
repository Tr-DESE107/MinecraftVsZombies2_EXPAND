using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MVZ2.Map
{
    public class MapButton : Selectable, IPointerDownHandler, IPointerClickHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
    {
        public void SetColor(Color color)
        {
            buttonRenderer.color = color;
        }
        public void SetText(string text)
        {
            buttonText.text = text;
        }
        public void SetBorderSprite(Sprite back, Sprite bottom, Sprite overlay)
        {
            defaultBorder.SetActive(false);
            customBorder.SetActive(true);
            borderBack.sprite = back;
            borderBottom.sprite = bottom;
            borderOverlay.sprite = overlay;
        }
        public void SetBorderSpriteToDefault()
        {
            defaultBorder.SetActive(true);
            customBorder.SetActive(false);
        }
        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            if (!IsInteractable())
                return;
            // Not left mouse nor first touch.
            if (eventData.pointerId != -1 && eventData.pointerId != 0)
                return;
            isPointerDown = true;
            UpdatePointer();
        }

        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {
            if (!IsInteractable())
                return;
            // Not left mouse nor first touch.
            if (eventData.pointerId != -1 && eventData.pointerId != 0)
                return;
            isPointerDown = false;
            UpdatePointer();
            OnClick?.Invoke();
        }

        void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
        {
            // Not left mouse nor first touch.
            if (eventData.pointerId != -1 && eventData.pointerId != 0)
                return;
            isPointerDown = false;
            UpdatePointer();
        }

        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {
            isPointerEnter = true;
            UpdatePointer();
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {
            isPointerEnter = false;
            UpdatePointer();
        }
        private void UpdatePointer()
        {
            animator.SetBool("Pressed", isPointerEnter && isPointerDown);
        }
        public event Action OnClick;
        private bool isPointerEnter;
        private bool isPointerDown;
        [SerializeField]
        private SpriteRenderer buttonRenderer;
        [SerializeField]
        private TextMeshPro buttonText;
        [SerializeField]
        private GameObject defaultBorder;
        [SerializeField]
        private GameObject customBorder;
        [SerializeField]
        private SpriteRenderer borderBack;
        [SerializeField]
        private SpriteRenderer borderBottom;
        [SerializeField]
        private SpriteRenderer borderOverlay;
    }
}
