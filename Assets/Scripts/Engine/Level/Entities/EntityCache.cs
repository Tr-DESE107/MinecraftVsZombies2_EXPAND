using Tools;
using UnityEngine;

namespace PVZEngine.Entities
{
    public class EntityCache
    {
        public void UpdateAll(Entity entity)
        {
            Faction = entity.GetFaction();
            Gravity = entity.GetGravity();
            Friction = entity.GetFriction();
            GroundLimitOffset = entity.GetGroundLimitOffset();
            VelocityDampen = entity.GetVelocityDampen();
            Size = entity.GetSize();
            BoundsOffset = entity.GetBoundsOffset();
            CollisionDetection = entity.GetCollisionDetection();
            CollisionSampleLength = entity.GetCollisionSampleLength();
        }
        public void UpdateProperty(Entity entity, string name, object value)
        {
            switch (name)
            {
                case EngineEntityProps.FACTION:
                    Faction = value.ToGeneric<int>();
                    break;
                case EngineEntityProps.GRAVITY:
                    Gravity = value.ToGeneric<float>();
                    break;
                case EngineEntityProps.FRICTION:
                    Friction = value.ToGeneric<float>();
                    break;
                case EngineEntityProps.GROUND_LIMIT_OFFSET:
                    GroundLimitOffset = value.ToGeneric<float>();
                    break;
                case EngineEntityProps.VELOCITY_DAMPEN:
                    VelocityDampen = value.ToGeneric<Vector3>();
                    break;
                case EngineEntityProps.SIZE:
                    Size = value.ToGeneric<Vector3>();
                    break;
                case EngineEntityProps.BOUNDS_OFFSET:
                    BoundsOffset = value.ToGeneric<Vector3>();
                    break;
                case EngineEntityProps.COLLISION_DETECTION:
                    CollisionDetection = value.ToGeneric<int>();
                    break;
                case EngineEntityProps.COLLISION_SAMPLE_LENGTH:
                    CollisionSampleLength = value.ToGeneric<float>();
                    break;
            }
        }
        public int Faction { get; private set; }
        public float Gravity { get; private set; }
        public float Friction { get; private set; }
        public float GroundLimitOffset { get; private set; }
        public Vector3 VelocityDampen { get; private set; }
        public Vector3 Size { get; private set; }
        public Vector3 BoundsOffset { get; private set; }
        public int CollisionDetection { get; private set; }
        public float CollisionSampleLength { get; private set; }
    }
}
