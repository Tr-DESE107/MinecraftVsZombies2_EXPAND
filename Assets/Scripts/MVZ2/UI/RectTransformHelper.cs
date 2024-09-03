using UnityEngine;

namespace MVZ2.UI
{
    public static class RectTransformHelper
    {
        public static Rect GetWorldRect(this RectTransform rectTrans)
        {
            var localRect = rectTrans.rect;
            var min = localRect.min;
            var max = localRect.max;
            Matrix4x4 matrix4x = rectTrans.localToWorldMatrix;
            var worldMin = matrix4x.MultiplyPoint(min);
            var worldMax = matrix4x.MultiplyPoint(max);
            return new Rect(worldMin, worldMax - worldMin);
        }
    }
}
