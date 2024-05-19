using UnityEngine;

namespace MVZ2
{
    public static class PositionHelper
    {
        public static Vector3 LawnToTrans(this Vector3 pos)
        {
            pos *= LAWN_TO_TRANS_SCALE;
            Vector3 vector = new Vector3(pos.x, pos.z + pos.y, pos.z);
            return vector;
        }
        public const float LAWN_TO_TRANS_SCALE = 0.01f;
    }
}
