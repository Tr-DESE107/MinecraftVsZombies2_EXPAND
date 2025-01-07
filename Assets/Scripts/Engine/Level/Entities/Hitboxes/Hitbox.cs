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
            var scale = Entity.GetScale();

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
            return cache.Intersects(other.cache);
        }
        public bool DoCollision(Hitbox other, Vector3 selfMotion, Vector3 otherMotion, int checkPoints, out Vector3 seperation)
        {
            checkPoints = Mathf.Max(checkPoints, 1);
            for (int p = 1; p <= checkPoints; p++)
            {
                var selfOffset = selfMotion * (p / (float)checkPoints - 1);
                var otherOffset = otherMotion * (p / (float)checkPoints - 1);

                var selfBounds = cache;
                selfBounds.center += selfOffset;
                var otherBounds = other.cache;
                otherBounds.center += otherOffset;
                
                if (selfBounds.Intersects(otherBounds))
                {
                    seperation = otherBounds.center - selfBounds.center;
                    return true;
                }
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
