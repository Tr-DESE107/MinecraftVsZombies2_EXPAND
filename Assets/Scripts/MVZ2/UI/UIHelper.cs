using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MVZ2.UI
{
    public static class UIHelper
    {
        public static Canvas GetRootCanvas(this RectTransform target)
        {
            var cacheList = new List<Canvas>();
            return target.GetRootCanvasNonAlloc(cacheList);
        }
        public static Canvas GetRootCanvasNonAlloc(this RectTransform target, List<Canvas> cacheList)
        {
            cacheList.Clear();
            target.GetComponentsInParent<Canvas>(true, cacheList);
            return cacheList.LastOrDefault();
        }
        public static void LimitInsideScreen(this RectTransform target)
        {
            var rootCanvas = target.GetRootCanvas();
            if (!rootCanvas)
                return;
            target.LimitInsideScreen(rootCanvas.transform);
        }
        public static void LimitInsideScreenNonAlloc(this RectTransform target, List<Canvas> cacheList)
        {
            var rootCanvas = target.GetRootCanvasNonAlloc(cacheList);
            if (!rootCanvas)
                return;
            target.LimitInsideScreen(rootCanvas.transform);
        }
        public static void LimitInsideScreen(this RectTransform target, Transform rootCanvasTransform)
        {
            var localRect = target.rect;
            var size = localRect.size;
            target.LimitInsideScreen(rootCanvasTransform, size);
        }
        public static void LimitInsideScreen(this RectTransform target, Transform rootCanvasTransform, Vector2 size)
        {
            RectTransform rootCanvasRectTransform = rootCanvasTransform as RectTransform;
            var rootCanvasWorldRect = rootCanvasRectTransform.GetWorldRect();
            var localMin = target.parent.InverseTransformPoint(rootCanvasWorldRect.min);
            var localMax = target.parent.InverseTransformPoint(rootCanvasWorldRect.max);

            var targetPos = target.localPosition;
            var xMin = localMin.x + size.x * target.pivot.x;
            var xMax = localMax.x - size.x * (1 - target.pivot.x);
            var yMin = localMin.y + size.y * target.pivot.y;
            var yMax = localMax.y - size.y * (1 - target.pivot.y);
            targetPos.x = Mathf.Clamp(targetPos.x, xMin, xMax);
            targetPos.y = Mathf.Clamp(targetPos.y, yMin, yMax);
            target.localPosition = targetPos;
        }
    }
}
