using UnityEngine;
using UnityEngine.EventSystems;

namespace MVZ2.UI
{
    public class CursorHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {
            _cursorSource = new HandlerCursorSource(gameObject, cursorType);
            CursorManager.AddSource(_cursorSource);
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {
            CursorManager.RemoveSource(_cursorSource);
            _cursorSource = null;
        }
        public CursorType cursorType = CursorType.Point;
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
