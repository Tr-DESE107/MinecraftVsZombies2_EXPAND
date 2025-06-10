using Tools;
using Tools.Mathematics;
using UnityEngine;

namespace PVZEngine.Entities
{
    public abstract class Hitbox
    {
        public Hitbox(Entity entity)
        {
            Entity = entity;
        }
        public void ReevaluateBounds()
        {
            var scale = Entity.GetFinalScale();

            Vector3 pivot = GetPivot();
            Vector3 size = GetSize();
            Vector3 offset = GetOffset();
            size.Scale(scale);
            size = size.Abs();

            var position = Vector3.Scale(offset, scale);
            cacheOffset = position;
            var center = position + Vector3.Scale(Vector3.one * 0.5f - pivot, size);

            cache = new Bounds(center, size);
        }
        public bool IsInSphere(Vector3 center, float radius)
        {
            var bounds = GetBounds();
            return MathTool.CollideBetweenCubeAndSphere(center, radius, bounds.center, bounds.size);
        }
        public Vector3 GetBoundsCenter()
        {
            return GetLocalCenter() + Entity.Position;
        }
        public Vector3 GetPosition()
        {
            return GetLocalOffset() + Entity.Position;
        }
        public Bounds GetBounds()
        {
            var bounds = GetLocalBounds();
            bounds.center += Entity.Position;
            return bounds;
        }
        public Vector3 GetBoundsSize()
        {
            return cache.size;
        }
        public Bounds GetLocalBounds()
        {
            return cache;
        }
        public Vector3 GetLocalCenter()
        {
            return cache.center;
        }
        public Vector3 GetLocalOffset()
        {
            return cacheOffset;
        }
        public bool Intersects(Hitbox other)
        {
            var bounds = GetBounds();
            var otherBounds = other.GetBounds();
            return bounds.IntersectsOptimized(otherBounds);
        }
        public bool DoCollision(Hitbox other, Vector3 offset, out Vector3 seperation)
        {
            var selfBounds = GetBounds();
            selfBounds.center += offset;
            var otherBounds = other.GetBounds();

            if (selfBounds.IntersectsOptimized(otherBounds))
            {
                seperation = otherBounds.center - selfBounds.center;
                return true;
            }
            seperation = Vector3.zero;
            return false;
        }
        public abstract Vector3 GetSize();
        public abstract Vector3 GetPivot();
        public abstract Vector3 GetOffset();
        public Entity Entity { get; }
        private Bounds cache;
        private Vector3 cacheOffset;
    }
}
