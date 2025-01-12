using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MVZ2
{
    public static class CameraHelper
    {
        public static Vector2 GetViewSize(this Camera camera)
        {
            float height = camera.orthographicSize * 2;
            return new Vector2(camera.aspect * height, height);
        }
        public static Rect GetViewRect(this Camera camera)
        {
            Vector2 size = camera.GetViewSize();
            Vector2 pos = camera.transform.position;
            return new Rect(pos - size * 0.5f, size);
        }
        public static float GetAspect(Vector2 resolution)
        {
            return resolution.x / resolution.y;
        }
    }
}
