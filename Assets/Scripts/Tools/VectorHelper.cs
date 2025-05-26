using UnityEngine;

namespace Tools
{
    public static class VectorHelper
    {
        public static Vector2 RotateClockwise(this Vector2 vector, float angle)
        {
            return Quaternion.AngleAxis(angle, Vector3.back) * vector;
        }
        public static Vector3 Abs(this Vector3 vector)
        {
            vector.x = Mathf.Abs(vector.x);
            vector.y = Mathf.Abs(vector.y);
            vector.z = Mathf.Abs(vector.z);
            return vector;
        }
    }
}
