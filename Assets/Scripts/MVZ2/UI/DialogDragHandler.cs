using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MVZ2.UI
{
    public class DialogDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
        {
            if (!eventData.pointerCurrentRaycast.isValid)
                return;
            dragTarget.GetComponentsInParent<Canvas>(true, canvasListCache);
            var rootCanvas = canvasListCache.LastOrDefault();
            if (rootCanvas == null)
                return;
            dragging = true;
            var worldPos = rootCanvas.worldCamera.ScreenToWorldPoint(eventData.position);
            dragTargetOffset = worldPos - dragTarget.position;
        }
        void IDragHandler.OnDrag(PointerEventData eventData)
        {
            if (!dragging)
                return;
            canvasListCache.Clear();
            dragTarget.GetComponentsInParent<Canvas>(true, canvasListCache);
            var rootCanvas = canvasListCache.LastOrDefault();
            if (rootCanvas == null)
                return;
            RectTransform rootCanvasRectTransform = rootCanvas.transform as RectTransform;

            var worldPos = rootCanvas.worldCamera.ScreenToWorldPoint(eventData.position);
            dragTarget.position = worldPos - dragTargetOffset;


            var rootCanvasWorldRect = rootCanvasRectTransform.GetWorldRect();
            var localRect = dragTarget.rect;
            var localMin = dragTarget.parent.InverseTransformPoint(rootCanvasWorldRect.min);
            var localMax = dragTarget.parent.InverseTransformPoint(rootCanvasWorldRect.max);

            var targetPos = dragTarget.localPosition;
            var xMin = localMin.x + localRect.size.x * dragTarget.pivot.x;
            var xMax = localMax.x - localRect.size.x * (1 - dragTarget.pivot.x);
            var yMin = localMin.y + localRect.size.y * dragTarget.pivot.y;
            var yMax = localMax.y - localRect.size.y * (1 - dragTarget.pivot.y);
            targetPos.x = Mathf.Clamp(targetPos.x, xMin, xMax);
            targetPos.y = Mathf.Clamp(targetPos.y, yMin, yMax);
            dragTarget.localPosition = targetPos;
        }
        void IEndDragHandler.OnEndDrag(PointerEventData eventData)
        {
            dragging = false;
        }
        [SerializeField]
        private RectTransform dragTarget;
        private bool dragging;
        private Vector3 dragTargetOffset;
        private List<Canvas> canvasListCache = new List<Canvas>();
    }
}
