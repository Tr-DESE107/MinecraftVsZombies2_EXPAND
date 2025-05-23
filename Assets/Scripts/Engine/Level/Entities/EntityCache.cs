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
        public void UpdateProperty(Entity entity, PropertyKey name, object beforeValue, object afterValue)
        {
            if (name == EngineEntityProps.FACTION)
            {
                Faction = afterValue.ToGeneric<int>();
            }
            else if (name == EngineEntityProps.GRAVITY)
            {
                Gravity = afterValue.ToGeneric<float>();
            }
            else if (name == EngineEntityProps.FRICTION)
            {
                Friction = afterValue.ToGeneric<float>();
            }
            else if (name == EngineEntityProps.GROUND_LIMIT_OFFSET)
            {
                GroundLimitOffset = afterValue.ToGeneric<float>();
            }
            else if (name == EngineEntityProps.VELOCITY_DAMPEN)
            {
                VelocityDampen = afterValue.ToGeneric<Vector3>();
            }
            else if (name == EngineEntityProps.SIZE)
            {
                Size = afterValue.ToGeneric<Vector3>();
                entity.UpdateCollision();
            }
            else if (name == EngineEntityProps.SCALE)
            {
                Scale = afterValue.ToGeneric<Vector3>();
                entity.UpdateCollision();
            }
            else if (name == EngineEntityProps.FLIP_X)
            {
                FlipX = afterValue.ToGeneric<bool>();
                entity.UpdateCollision();
            }
            else if (name == EngineEntityProps.BOUNDS_PIVOT)
            {
                BoundsPivot = afterValue.ToGeneric<Vector3>();
                entity.UpdateCollision();
            }
            else if (name == EngineEntityProps.COLLISION_DETECTION)
            {
                CollisionDetection = afterValue.ToGeneric<int>();
                entity.UpdateCollision();
            }
            else if (name == EngineEntityProps.COLLISION_SAMPLE_LENGTH)
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
