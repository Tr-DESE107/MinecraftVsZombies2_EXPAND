using MVZ2.Rendering;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MVZ2.UI
{
    public class CursorHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private void OnDisable()
        {
            if (isHovered)
            {
                isHovered = false;
                UpdateCursor();
            }
        }
        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {
            if (Interactable)
            {
                isHovered = true;
                UpdateCursor();
            }
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {
            if (isHovered)
            {
                isHovered = false;
                UpdateCursor();
            }
        }
        private void Enter()
        {
            if (_cursorSource == null)
            {
                _cursorSource = new HandlerCursorSource(gameObject, cursorType);
                CursorManager.AddSource(_cursorSource);
            }
        }
        private void Exit()
        {
            if (_cursorSource != null)
            {
                CursorManager.RemoveSource(_cursorSource);
                _cursorSource = null;
            }
        }
        private void UpdateCursor()
        {
            if (isHovered && Interactable)
            {
                Enter();
            }
            else
            {
                Exit();
            }
        }
        public bool Interactable
        {
            get
            {
                return (!trackSelectable || trackSelectable.interactable) && interactable;
            }
            set
            {
                interactable = value;
                UpdateCursor();
            }
        }
        public CursorType cursorType = CursorType.Point;
        [SerializeField]
        private Selectable trackSelectable;
        private bool isHovered;
        private bool interactable = true;
        private HandlerCursorSource _cursorSource;
    }
    public class HandlerCursorSource : ICursorSource
    {
        public HandlerCursorSource(GameObject target, CursorType type, int priority = 0)
        {
            this.target = target;
            this.type = type;
            this.priority = priority;
        }

        public bool IsValid()
        {
            return target && target.activeInHierarchy;
        }

        public GameObject target;
        private int priority;
        public int Priority => priority;
        private CursorType type;
        public CursorType CursorType => type;
    }
}
