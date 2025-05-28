using MVZ2.Vanilla.Detections;
using MVZ2.Vanilla.Level;
using PVZEngine.Entities;
using UnityEngine;

namespace MVZ2.GameContent.Detections
{
    public class LaneDetector : Detector
    {
        public LaneDetector(float ySize, float zSize)
        {
            this.ySize = ySize;
            this.zSize = zSize;
        }
        protected override Bounds GetDetectionBounds(Entity self)
        {
            var minX = VanillaLevelExt.LEFT_BORDER;
            var maxX = VanillaLevelExt.RIGHT_BORDER;
            var sizeY = ySize;
            var sizeZ = zSize;
            var center = self.GetCenter();
            center.x = (minX + maxX) * 0.5f;
            return new Bounds(center, new Vector3(maxX - minX, sizeY, sizeZ));
        }
        private float ySize;
        private float zSize;
    }
}
