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
        public void Update()
        {
            var scale = Entity.Cache.Scale;

            Vector3 offset = GetOffset();
            offset.Scale(scale);

            Vector3 size = GetSize();
            size.Scale(scale);

            var center = Entity.Position + Vector3.up * (0.5f * size.y) + offset;

            size.x = Mathf.Abs(size.x);
            size.y = Mathf.Abs(size.y);
            size.z = Mathf.Abs(size.z);
            cache = new Bounds(center, size);
        }
        public Vector3 GetBoundsCenter()
        {
            return cache.center;
        }
        public Bounds GetBounds()
        {
            return cache;
        }
        public Vector3 GetBoundsSize()
        {
            return cache.size;
        }
        public bool Intersects(Hitbox other)
        {
            return cache.Intersects(cache);
        }
        public bool DoCollision(Hitbox other, Vector3 offset, out Vector3 seperation)
        {
            var selfBounds = cache;
            selfBounds.center += offset;
            var otherBounds = other.cache;
                
            if (selfBounds.Intersects(otherBounds))
            {
                seperation = otherBounds.center - selfBounds.center;
                return true;
            }
            seperation = Vector3.zero;
            return false;
        }
        public abstract Vector3 GetSize();
        public abstract Vector3 GetOffset();
        public abstract SerializableHitbox ToSerializable();
        public Entity Entity { get; }
        private Bounds cache;
    }
}
