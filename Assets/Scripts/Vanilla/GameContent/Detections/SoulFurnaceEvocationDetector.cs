using MVZ2.Vanilla.Detections;
using PVZEngine.Entities;

namespace MVZ2.GameContent.Detections
{
    public class SoulFurnaceEvocationDetector : Detector
    {
        public override bool IsInRange(Entity self, Entity target)
        {
            return Detection.IsInFrontOf(self, target, 0) &&
                Detection.CoincidesYDown(target, self.GetBounds().max.y);
        }
    }
}
