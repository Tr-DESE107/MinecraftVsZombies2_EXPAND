using UnityEngine;
using UnityEngine.EventSystems;

namespace MVZ2.UI
{
    public class CursorHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private void OnDisable()
        {
            Exit();
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
            get => interactable;
            set
            {
                interactable = value;
                UpdateCursor();
            }
        }
        public CursorType cursorType = CursorType.Point;
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
