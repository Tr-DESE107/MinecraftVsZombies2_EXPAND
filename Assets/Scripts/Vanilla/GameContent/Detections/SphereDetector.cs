using MVZ2.Vanilla.Detections;
using MVZ2.Vanilla.Entities;
using PVZEngine.Entities;
using UnityEngine;

namespace MVZ2.GameContent.Detections
{
    public class SphereDetector : Detector
    {
        public SphereDetector(float radius)
        {
            this.radius = radius;
        }
        protected override Bounds GetDetectionBounds(Entity self)
        {
            var sizeX = radius * 2;
            var sizeY = radius * 2;
            var sizeZ = radius * 2;
            var center = self.GetCenter();
            return new Bounds(center, new Vector3(sizeX, sizeY, sizeZ));
        }
        private float radius;
    }
}
