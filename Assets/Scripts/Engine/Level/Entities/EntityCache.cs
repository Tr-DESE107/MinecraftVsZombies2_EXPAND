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
            CanUnderGround = entity.CanUnderGround();
            VelocityDampen = entity.GetVelocityDampen();
            Size = entity.GetSize();
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
                case EngineEntityProps.CAN_UNDER_GROUND:
                    CanUnderGround = value.ToGeneric<bool>();
                    break;
                case EngineEntityProps.VELOCITY_DAMPEN:
                    VelocityDampen = value.ToGeneric<Vector3>();
                    break;
                case EngineEntityProps.SIZE:
                    Size = value.ToGeneric<Vector3>();
                    break;
            }
        }
        public int Faction { get; private set; }
        public float Gravity { get; private set; }
        public float Friction { get; private set; }
        public bool CanUnderGround { get; private set; }
        public Vector3 VelocityDampen { get; private set; }
        public Vector3 Size { get; private set; }
    }
}
