using UnityEngine;

namespace MVZ2
{
    public static class PositionHelper
    {
        public static Vector3 LawnToTrans(this Vector3 pos, float xOffset = 0, float yOffset = 0, float zOffset = 0)
        {
            float pixelUnit = 0.01f;
            pos *= pixelUnit;
            Vector3 vector = new Vector3(pos.x + xOffset, pos.z + pos.y + yOffset, pos.z + zOffset);
            return vector;
        }

        public static Vector3 LawnToTrans(this Vector3 pos, Vector3 offset)
        {
            float pixelUnit = 0.01f;
            pos *= pixelUnit;
            Vector3 vector = new Vector3(pos.x + offset.x, pos.z + pos.y + offset.y, pos.z + offset.z);
            return vector;
        }
    }
}
