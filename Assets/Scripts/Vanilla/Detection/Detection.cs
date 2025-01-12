using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using MVZ2.Vanilla.Entities;
using PVZEngine.Entities;
using PVZEngine.Level;
using Tools.Mathematics;
using UnityEngine;
using static System.Collections.Specialized.BitVector32;

namespace MVZ2.Vanilla.Detections
{
    public static class Detection
    {
        public static bool Intersects(this Hitbox self, Hitbox other)
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
                target.MainHitbox.CoincidesYDown(self.Position.y + heightOffset) &&
                IsZCoincide(self.Position.z, projectileZSpan, target.Position.z, target.GetSize().z);
        }

        public static bool InFrontShooterRange(Entity self, Entity target, float projectileZSpan, float heightOffset, float frontRange)
        {
            return IsInFrontOf(self, target, 0, frontRange) &&
                target.MainHitbox.CoincidesYDown(self.Position.y + heightOffset) &&
                IsZCoincide(self.Position.z, projectileZSpan, target.Position.z, target.GetSize().z);
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
                return other.Position.x <= self.Position.x - nearRange && other.Position.x > self.Position.x - farRange;
            }
            else
            {
                return other.Position.x >= self.Position.x + nearRange && other.Position.x < self.Position.x + farRange;
            }
        }

        public static bool IsInFrontOf(Entity self, Entity other, float rangeStart = 0)
        {
            if (self.IsFacingLeft())
            {
                return other.Position.x <= self.Position.x - rangeStart;
            }
            else
            {
                return other.Position.x >= self.Position.x + rangeStart;
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
        public static bool CoincidesYDown(this Hitbox hitbox, float y)
        {
            return hitbox.GetBounds().min.y < y;
        }

        public static bool CoincidesYUp(this Hitbox hitbox, float y)
        {
            return hitbox.GetBounds().max.y > y;
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

        public static bool IsInSphere(this Hitbox hitbox, Vector3 center, float radius)
        {
            var bounds = hitbox.GetBounds();
            return MathTool.CollideBetweenCubeAndSphere(center, radius, bounds.center, bounds.size);
        }
        public static bool IsInSphere(this EntityCollider collider, Vector3 center, float radius)
        {
            for (int i = 0; i < collider.GetHitboxCount(); i++)
            {
                var hitbox = collider.GetHitbox(i);
                if (hitbox.IsInSphere(center, radius))
                    return true;
            }
            return false;
        }
        public static IEnumerable<EntityCollider> GetCollidersInSphere(this Entity entity, Vector3 center, float radius)
        {
            return entity.GetEnabledColliders().Where(g => g.IsInSphere(center, radius));
        }
        private static IEnumerable<EntityCollider> Overlap(this LevelEngine level, int faction, int hostileMask, int friendlyMask, Predicate<Hitbox> predicate)
        {
            if (predicate == null)
                yield break;
            var entities = level.FindEntities(e => EntityCollisionHelper.CanCollide(e.IsHostile(faction) ? hostileMask : friendlyMask, e));
            foreach (var entity in entities)
            {
                foreach (var collider in entity.GetEnabledColliders())
                {
                    for (int i = 0; i < collider.GetHitboxCount(); i++)
                    {
                        var hitbox = collider.GetHitbox(i);
                        if (predicate(hitbox))
                        {
                            yield return collider;
                            break;
                        }
                    }
                }
            }
        }
        public static IEnumerable<EntityCollider> OverlapBox(this LevelEngine level, Vector3 center, Vector3 size, int faction, int hostileMask, int friendlyMask)
        {
            return level.Overlap(faction, hostileMask, friendlyMask, h => Detection.Intersects(center, size, h.GetBoundsCenter(), h.GetBoundsSize()));
        }
        public static IEnumerable<EntityCollider> OverlapCylinder(this LevelEngine level, Vector3 center, float radius, float height, int faction, int hostileMask, int friendlyMask)
        {
            var cylinder = new Cylinder(Axis.Y, center, height, radius);
            return level.Overlap(faction, hostileMask, friendlyMask, h => MathTool.CollideBetweenCudeAndCylinder(cylinder, h.GetBounds()));
        }
    }
}