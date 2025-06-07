using Tools;
using UnityEngine;

namespace PVZEngine.Entities
{
    public class EntityCache
    {
        public void UpdateAll(Entity entity)
        {
            Faction = entity.GetProperty<int>(EngineEntityProps.FACTION);
            Gravity = entity.GetGravity();
            Friction = entity.GetFriction();
            GroundLimitOffset = entity.GetGroundLimitOffset();
            VelocityDampen = entity.GetVelocityDampen();
            Size = entity.GetSize();
            Scale = entity.GetScale();
            FlipX = entity.IsFlipX();
            BoundsPivot = entity.GetBoundsPivot();
            CollisionDetection = entity.GetCollisionDetection();
            CollisionSampleLength = entity.GetCollisionSampleLength();
            entity.UpdateCollision();
        }
        public void UpdateProperty(Entity entity, IPropertyKey name, object beforeValue, object afterValue)
        {
            if (EngineEntityProps.FACTION.Equals(name))
            {
                Faction = afterValue.ToGeneric<int>();
            }
            else if (EngineEntityProps.GRAVITY.Equals(name))
            {
                Gravity = afterValue.ToGeneric<float>();
            }
            else if (EngineEntityProps.FRICTION.Equals(name))
            {
                Friction = afterValue.ToGeneric<float>();
            }
            else if (EngineEntityProps.GROUND_LIMIT_OFFSET.Equals(name))
            {
                GroundLimitOffset = afterValue.ToGeneric<float>();
            }
            else if (EngineEntityProps.VELOCITY_DAMPEN.Equals(name))
            {
                VelocityDampen = afterValue.ToGeneric<Vector3>();
            }
            else if (EngineEntityProps.SIZE.Equals(name))
            {
                Size = afterValue.ToGeneric<Vector3>();
                entity.UpdateCollisionSize();
            }
            else if (EngineEntityProps.SCALE.Equals(name))
            {
                Scale = afterValue.ToGeneric<Vector3>();
                entity.UpdateCollisionSize();
            }
            else if (EngineEntityProps.FLIP_X.Equals(name))
            {
                FlipX = afterValue.ToGeneric<bool>();
                entity.UpdateCollisionSize();
            }
            else if (EngineEntityProps.BOUNDS_PIVOT.Equals(name))
            {
                BoundsPivot = afterValue.ToGeneric<Vector3>();
                entity.UpdateCollisionSize();
            }
            else if (EngineEntityProps.COLLISION_DETECTION.Equals(name))
            {
                CollisionDetection = afterValue.ToGeneric<int>();
                entity.UpdateCollisionDetection();
            }
            else if (EngineEntityProps.COLLISION_SAMPLE_LENGTH.Equals(name))
            {
                CollisionSampleLength = afterValue.ToGeneric<float>();
            }
        }
        public Vector3 GetFinalScale()
        {
            var scale = Scale;
            scale.x *= FlipX ? -1 : 1;
            return scale;
        }
        public int Faction { get; private set; }
        public bool FlipX { get; private set; }
        public float Gravity { get; private set; }
        public float Friction { get; private set; }
        public float GroundLimitOffset { get; private set; }
        public Vector3 VelocityDampen { get; private set; }
        public Vector3 Size { get; private set; }
        public Vector3 Scale { get; private set; }
        public Vector3 BoundsPivot { get; private set; }
        public int CollisionDetection { get; private set; }
        public float CollisionSampleLength { get; private set; }
    }
}
