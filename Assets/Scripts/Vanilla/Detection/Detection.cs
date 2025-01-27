using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using MVZ2.GameContent.Effects;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Grids;
using MVZ2.Vanilla.Level;
using PVZEngine.Entities;
using PVZEngine.Grids;
using PVZEngine.Level;
using Tools.Mathematics;
using UnityEngine;
using static System.Collections.Specialized.BitVector32;
using static UnityEngine.EventSystems.EventTrigger;

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
        public static bool IsInFrontOf(Entity self, float x, float rangeStart = 0)
        {
            if (self.IsFacingLeft())
            {
                return x <= self.Position.x - rangeStart;
            }
            else
            {
                return x >= self.Position.x + rangeStart;
            }
        }
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
            return IsInFrontOf(self, other.Position.x, rangeStart);
        }

        public static bool IsXCoincide(float x1, float xLength1, float x2, float xLength2)
        {
            float extent1 = xLength1 / 2;
            float extent2 = xLength2 / 2;
            return MathTool.DoRangesIntersect(x1 - extent1, x1 + extent1, x2 - extent2, x2 + extent2);
        }
        #endregion

        #region Y related
        public static bool CoincidesYDown(this Bounds bounds, float y)
        {
            return bounds.min.y < y;
        }
        public static bool CoincidesYDown(this Hitbox hitbox, float y)
        {
            return hitbox.GetBounds().min.y < y;
        }

        public static bool CoincidesYUp(this Bounds bounds, float y)
        {
            return bounds.max.y > y;
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
        public static IEnumerable<EntityCollider> Overlap(this LevelEngine level, Rect filterRect, int faction, int hostileMask, int friendlyMask, Predicate<Hitbox> predicate)
        {
            if (predicate == null)
                yield break;
            var entityColliders = new List<EntityCollider>();
            var totalMask = hostileMask | friendlyMask;
            level.FindCollidersRange(totalMask, filterRect, entityColliders);
            foreach (var collider in entityColliders)
            {
                var entity = collider.Entity;
                var mask = entity.IsHostile(faction) ? hostileMask : friendlyMask;
                if (!EntityCollisionHelper.CanCollide(mask, entity))
                    continue;
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
        public static void OverlapNonAlloc(this LevelEngine level, Rect filterRect, int faction, int hostileMask, int friendlyMask, Predicate<Hitbox> predicate, HashSet<EntityCollider> results)
        {
            if (predicate == null)
                return;
            var entityColliders = new List<EntityCollider>();
            var totalMask = hostileMask | friendlyMask;
            level.FindCollidersRange(totalMask, filterRect, entityColliders);
            foreach (var collider in entityColliders)
            {
                var entity = collider.Entity;
                var mask = entity.IsHostile(faction) ? hostileMask : friendlyMask;
                if (!EntityCollisionHelper.CanCollide(mask, entity))
                    continue;
                for (int i = 0; i < collider.GetHitboxCount(); i++)
                {
                    var hitbox = collider.GetHitbox(i);
                    if (predicate(hitbox))
                    {
                        results.Add(collider);
                        break;
                    }
                }
            }
        }
        public static IEnumerable<EntityCollider> OverlapBox(this LevelEngine level, Vector3 center, Vector3 size, int faction, int hostileMask, int friendlyMask)
        {
            var min = center - size * 0.5f;
            var filterRect = new Rect(min.x, min.z, size.x, size.y);
            return level.Overlap(filterRect, faction, hostileMask, friendlyMask, h => Detection.Intersects(center, size, h.GetBoundsCenter(), h.GetBoundsSize()));
        }
        public static IEnumerable<EntityCollider> OverlapCylinder(this LevelEngine level, Vector3 center, float radius, float height, int faction, int hostileMask, int friendlyMask)
        {
            var min = center - Vector3.one * radius;
            var filterRect = new Rect(min.x, min.z, radius * 2, radius * 2);
            var cylinder = new Cylinder(Axis.Y, center, height, radius);
            return level.Overlap(filterRect, faction, hostileMask, friendlyMask, h => MathTool.CollideBetweenCubeAndCylinder(cylinder, h.GetBounds()));
        }
        public static IEnumerable<EntityCollider> OverlapSphere(this LevelEngine level, Vector3 center, float radius, int faction, int hostileMask, int friendlyMask)
        {
            var min = center - Vector3.one * radius;
            var filterRect = new Rect(min.x, min.z, radius * 2, radius * 2);
            return level.Overlap(filterRect, faction, hostileMask, friendlyMask, h => MathTool.CollideBetweenCubeAndSphere(center, radius, h.GetBoundsCenter(), h.GetBoundsSize()));
        }
        public static void OverlapSphereNonAlloc(this LevelEngine level, Vector3 center, float radius, int faction, int hostileMask, int friendlyMask, HashSet<EntityCollider> results)
        {
            var min = center - Vector3.one * radius;
            var filterRect = new Rect(min.x, min.z, radius * 2, radius * 2);
            level.OverlapNonAlloc(filterRect, faction, hostileMask, friendlyMask, h => MathTool.CollideBetweenCubeAndSphere(center, radius, h.GetBoundsCenter(), h.GetBoundsSize()), results);
        }
        public static void OverlapGridGroundNonAlloc(this LevelEngine level, int column, int lane, int faction, int hostileMask, int friendlyMask, HashSet<EntityCollider> results)
        {
            var minX = level.GetColumnX(column);
            var minZ = level.GetLaneZ(lane);
            var filterRect = new Rect(minX, minZ, level.GetGridWidth(), level.GetGridHeight());
            level.OverlapNonAlloc(filterRect, faction, hostileMask, friendlyMask, h => h.Entity.GetRelativeY() <= 0, results);
        }
    }
}