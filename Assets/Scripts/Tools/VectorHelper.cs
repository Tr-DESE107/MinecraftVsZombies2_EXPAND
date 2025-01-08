using UnityEngine;

namespace Tools
{
    public static class VectorHelper
    {
        public static Vector2 RotateClockwise(this Vector2 vector, float angle)
        {
            return Quaternion.AngleAxis(angle, Vector3.forward) * vector;
        }
    }
}
