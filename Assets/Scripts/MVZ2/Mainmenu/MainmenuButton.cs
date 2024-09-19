using System;
using MVZ2.UI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MVZ2.Mainmenu
{
    [RequireComponent(typeof(CursorHandler))]
    public class MainmenuButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        private void Awake()
        {
            cursorHandler = GetComponent<CursorHandler>();
        }
        private void OnEnable()
        {
            UpdateSprite();
        }
        private void OnDisable()
        {
            if (isHovered)
            {
                isHovered = false;
                UpdateSprite();
            }
        }
        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {
            if (Interactable)
                OnClick?.Invoke();
        }

        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {
            if (Interactable)
            {
                isHovered = true;
                UpdateSprite();
            }
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {
            if (isHovered)
            {
                isHovered = false;
                UpdateSprite();
            }
        }
        private void UpdateSprite()
        {
            var sprKey = normalSprite;
            if (!Interactable)
            {
                sprKey = disabledSprite;
            }
            else if (isHovered)
            {
                sprKey = hoveredSprite;
            }
            var spr = main.LanguageManager.GetSprite(sprKey);
            spriteRenderer.sprite = spr;
        }

        public event Action OnClick;
        public bool Interactable
        {
            get => interactable;
            set
            {
                interactable = value;
                cursorHandler.Interactable = value;
                UpdateSprite();
            }
        }
        private MainManager main => MainManager.Instance;
        private bool isHovered;
        private CursorHandler cursorHandler;
        [SerializeField]
        private bool interactable = true;
        [SerializeField]
        private SpriteRenderer spriteRenderer;
        [SerializeField]
        private Sprite normalSprite;
        [SerializeField]
        private Sprite hoveredSprite;
        [SerializeField]
        private Sprite disabledSprite;
    }
}
