using MVZ2.Vanilla.Contraptions;
using MVZ2.Vanilla.Detections;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Level;
using PVZEngine.Entities;
using UnityEngine;

namespace MVZ2.GameContent.Detections
{
    public class WitherDetector : Detector
    {
        public WitherDetector()
        {
        }
        protected override Bounds GetDetectionBounds(Entity self)
        {
            var source = self.Position;
            float sizeX = VanillaLevelExt.RIGHT_BORDER - VanillaLevelExt.LEFT_BORDER;
            float sizeY = 800;
            float sizeZ = self.Level.GetGridTopZ() - self.Level.GetGridBottomZ();
            float centerX = (VanillaLevelExt.LEFT_BORDER + VanillaLevelExt.RIGHT_BORDER) * 0.5f;
            float centerY = 400;
            float centerZ = (self.Level.GetGridTopZ() + self.Level.GetGridBottomZ()) * 0.5f;
            return new Bounds(new Vector3(centerX, centerY, centerZ), new Vector3(sizeX, sizeY, sizeZ));
        }
    }
}
