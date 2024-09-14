using Tools;
using UnityEngine;

namespace PVZEngine.Level
{
    public static class Detection
    {
        public static bool Intersects(Entity self, Entity other)
        {
            Bounds selfBounds = self.GetBounds();
            Bounds otherBounds = other.GetBounds();
            return selfBounds.Intersects(otherBounds);
        }
        public static bool Intersects(Vector3 center1, Vector3 size1, Vector3 center2, Vector3 size2)
        {
            var bounds1 = new Bounds(center1, size1);
            var bounds2 = new Bounds(center2, size2);
            return bounds1.Intersects(bounds2);
        }
        public static bool InFrontShooterRange(Entity self, Entity target, float projectileZSpan, float heightOffset)
        {
            return IsInFrontOf(self, target) &&
                IsBelowOf(self, target, heightOffset) &&
                IsZCoincide(self.Pos.z, projectileZSpan, target.Pos.z, target.GetSize().z);
        }

        public static bool InFrontShooterRange(Entity self, Entity target, float projectileZSpan, float heightOffset, float frontRange)
        {
            return IsInFrontOf(self, target, 0, frontRange) &&
                IsBelowOf(self, target, heightOffset) &&
                IsZCoincide(self.Pos.z, projectileZSpan, target.Pos.z, target.GetSize().z);
        }

        public static bool CanDetect(Entity entity)
        {
            return !entity.IsInvisible();
        }

        #region X related
        public static bool IsInFrontOf(Entity self, Entity other, float rangeStart, float rangeLength)
        {
            float nearRange = rangeStart;
            float farRange = rangeStart + rangeLength;
            if (self.IsFacingLeft())
            {
                return other.Pos.x <= self.Pos.x - nearRange && other.Pos.x > self.Pos.x - farRange;
            }
            else
            {
                return other.Pos.x >= self.Pos.x + nearRange && other.Pos.x < self.Pos.x + farRange;
            }
        }

        public static bool IsInFrontOf(Entity self, Entity other, float rangeStart = 0)
        {
            if (self.IsFacingLeft())
            {
                return other.Pos.x <= self.Pos.x - rangeStart;
            }
            else
            {
                return other.Pos.x >= self.Pos.x + rangeStart;
            }
        }

        public static bool IsXCoincide(float x1, float xLength1, float x2, float xLength2)
        {
            float extent1 = xLength1 / 2;
            float extent2 = xLength2 / 2;
            return MathTool.DoRangesIntersect(x1 - extent1, x1 + extent1, x2 - extent2, x2 + extent2);
        }
        #endregion

        #region Y related
        public static bool IsBelowOf(Entity self, Entity other, float offset = 0)
        {
            return other.Pos.y < self.Pos.y + offset;
        }

        public static bool IsOverOf(Entity self, Entity other, float offset = 0)
        {
            return other.Pos.y > self.Pos.y + offset;
        }

        public static bool IsYCoincide(float y1, float yLength1, float y2, float yLength2)
        {
            return MathTool.DoRangesIntersect(y1, y1 + yLength1, y2, y2 + yLength2);
        }
        #endregion

        #region Z related
        public static bool IsInSameRow(Entity self, Entity other)
        {
            return self.GetLane() == other.GetLane();
        }
        public static bool IsZCoincide(float z1, float zLength1, float z2, float zLength2)
        {
            float extent1 = zLength1 / 2;
            float extent2 = zLength2 / 2;
            return MathTool.DoRangesIntersect(z1 - extent1, z1 + extent1, z2 - extent2, z2 + extent2);
        }
        #endregion

        public static bool IsInSphere(Entity entity, Vector3 center, float radius)
        {
            var bounds = entity.GetBounds();
            return MathTool.CollideBetweenCubeAndSphere(center, radius, bounds.center, bounds.size);
        }
    }
}