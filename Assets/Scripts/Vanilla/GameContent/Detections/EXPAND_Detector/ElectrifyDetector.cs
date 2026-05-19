using MVZ2.GameContent.Shells;
using MVZ2.Vanilla.Detections;
using MVZ2.Vanilla.Entities;
using PVZEngine.Entities;
using UnityEngine;

namespace MVZ2.GameContent.Detections
{
    public class ElectrifyDetector : Detector
    {
        public ElectrifyDetector(float extraHeight)
        {
            canDetectInvisible = true;
            this.extraHeight = extraHeight;
        }
        protected override Bounds GetDetectionBounds(Entity self)
        {
            var bounds = self.GetBounds();
            bounds.max += Vector3.up * extraHeight;
            return bounds;
        }
        //protected override bool ValidateCollider(DetectionParams param, IEntityCollider collider)
        //{
        //    if (!base.ValidateCollider(param, collider))
        //        return false;

        //    var target = collider.Entity;
        //    var shellID = target.GetShellID();

        //    // 只检测金属外壳的投射物  
        //    return shellID != null && shellID == VanillaShellID.metal;
        //}
        private float extraHeight;
    }
}